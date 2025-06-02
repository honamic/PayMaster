using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Models;
using System.Text;
using System.Globalization;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class ZarinPalPaymentProvider(
    IHttpClientFactory httpClientFactory,
    ILogger<ZarinPalPaymentProvider> logger)
    : PaymentGatewayProviderBase
{
    private ZarinPalConfigurations _configurations = new ZarinPalConfigurations();

    public override void Configure(string providerConfiguration)
    {
        var zarinPalConfigurations = JsonSerializer.Deserialize<ZarinPalConfigurations>(providerConfiguration);

        _configurations = zarinPalConfigurations ??
                         throw new ArgumentNullException(nameof(providerConfiguration));
    }

    public override async Task<CreateResult> CreateAsync(CreateRequest prePayRequest)
    {
        var result = new CreateResult();

        try
        {
            var url = new Uri(new Uri(_configurations.ApiAddress), Constants.PAYMENT_REQUEST_URL);

            var client = httpClientFactory.CreateClient(Constants.HttpClientName);
            var apiRequest = new PaymentRequest
            {
                merchant_id = _configurations.MerchantId,
                amount = prePayRequest.Amount,
                callback_url = prePayRequest.CallbackUrl,
                description = prePayRequest.GatewayNote ?? $"درخواست شماره {prePayRequest.UniqueRequestId}",
                order_id = $"{prePayRequest.UniqueRequestId}",
                MetaData = new PaymentRequestMetaData
                {
                    mobile = prePayRequest.MobileNumber,
                    email = prePayRequest.Email,
                    card_pan = null
                }
            };

            result.LogData.Start(apiRequest, url.ToString());

            var apiResponse = await client.PostAsJsonAsync(url, apiRequest);

            result.LogData.End();

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.SetResponse(rawResponse);

            if (apiResponse.IsSuccessStatusCode)
            {
                var zarinPalResult = JsonSerializer.Deserialize<ZarinPalResult<PaymentRequestResponse>>
                    (rawResponse);
                if (zarinPalResult is not { data.Code: 100 })
                {
                    result.StatusDescription = GetDescriptionFromCode(zarinPalResult?.data?.Code);
                    return result;
                }

                var payUrl = $"{_configurations.PayUrl.TrimEnd('/')}/{zarinPalResult.data.Authority}";
                result.PayUrl = payUrl;
                result.PayVerb = PayVerb.Get;
                result.CreateReference = zarinPalResult.data.Authority;
                result.Success = true;
                return result;
            }
            else
            {
                var zarinPalResult = JsonSerializer.Deserialize<ZarinPalResult<PaymentRequestResponse?>>(rawResponse);
                var error = zarinPalResult?.errors?.errors?.FirstOrDefault();

                if (error != null)
                {
                    result.StatusDescription = error.message + " | " + GetDescriptionFromCode(error.code);
                }
                else
                {
                    result.StatusDescription = $"Status Code:{apiResponse.StatusCode}";
                }
            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            logger.LogError(ex, "PrePay Failed");
        }

        return result;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<CallBackDataModel>(callBackJsonValue);

            if (callbackData is { Status: "OK" })
            {
                result.UniqueRequestId = null;
                result.CreateReference = callbackData.Authority;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else if (callbackData is { Status: "NOK" })
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Canceled;
            }
            else
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Other;
                result.Error = $"Status is not valid! [{callbackData?.Status}]";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override async Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerifyResult();
        try
        {
            var callbackData = (CallBackDataModel?)request.CallBackData;
            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                return result;
            }

            var verificationRequest = new PaymentVerificationRequest
            {
                merchant_id = _configurations.MerchantId,
                amount = request.PaymentInfo.Amount,
                authority = callbackData!.Authority
            };

            var json = JsonSerializer.Serialize(verificationRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = httpClientFactory.CreateClient(Constants.HttpClientName);
            var url = new Uri(new Uri(_configurations.ApiAddress), Constants.PAYMENT_VERIFICATION_URL);

            result.VerifyLogData.Start(verificationRequest, url.ToString());

            var response = await client.PostAsync(url, content);

            result.VerifyLogData.End();

            var responseString = await response.Content.ReadAsStringAsync();

            result.VerifyLogData.SetResponse(responseString);

            var paymentVerificationResponse = JsonSerializer.Deserialize<ZarinPalResult<PaymentVerificationResponse>>
                (responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (paymentVerificationResponse is null
                || (paymentVerificationResponse.data.code != 100
                && paymentVerificationResponse.data.code != 101)
                )
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                result.StatusDescription = GetDescriptionFromCode(paymentVerificationResponse?.data.code);
                return result;
            }

            result.Success = true;
            result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
            {
                Pan = paymentVerificationResponse.data.card_pan,
                SuccessReference = $"{paymentVerificationResponse.data.ref_id}",
                ReferenceRetrievalNumber = null,
                TrackingNumber = paymentVerificationResponse.data.ref_id.ToString(CultureInfo.InvariantCulture),
                TerminalId = null,
                MerchantId = null,
            };
        }
        catch (Exception ex)
        {
            result.StatusDescription = ex.Message;
            logger.LogError(ex, "Verify Failed");
        }

        return result;
    }


    #region Private methods

    private string? GetDescriptionFromCode(int? dataCode)
    {
        //TODO: complete these codes
        //https://www.zarinpal.com/docs/paymentGateway/errorList.html

        return dataCode switch
        {
            null => "خطای نامشخص | کد null",
            100 => "عملیات موفق | کد 100",
            101 => "تراکنش وریفای شده است | کد 101",
            -9 => "خطای اعتبار سنجی | کد -9",
            -10 => "ای پی یا مرچنت كد پذیرنده صحیح نیست | کد -10",
            -11 => "مرچنت کد فعال نیست، لطفاً با امور مشتریان تماس بگیرید | کد -11",
            -12 => "تعداد تلاش بیش از حد مجاز، لطفاً بعداً تلاش کنید | کد -12",
            -15 => "درگاه پرداخت به حالت تعلیق درآمده است | کد -15",
            -16 => "سطح تایید پذیرنده پایین‌تر از سطح نقره‌ای است | کد -16",
            -17 => "محدودیت پذیرنده در سطح آبی | کد -17",
            -18 => "آدرس ارجاع با دامنه ثبت‌شده مطابقت ندارد | کد -18",
            -19 => "امکان ایجاد تراکنش برای این ترمینال وجود ندارد | کد -19",
            -30 => "پذیرنده اجازه دسترسی به سرویس تسویه اشتراکی شناور را ندارد | کد -30",
            -31 => "حساب بانکی تسویه را به پنل اضافه کنید | کد -31",
            -32 => "مبلغ وارد شده از حد مجاز بیشتر است | کد -32",
            -33 => "درصدهای وارد شده صحیح نیست | کد -33",
            -34 => "مبلغ وارد شده از مبلغ کل تراکنش بیشتر است | کد -34",
            -50 => "مبلغ پرداخت شده با مقدار مبلغ ارسال‌شده متفاوت است | کد -50",
            -51 => "پرداخت ناموفق | کد -51",
            -52 => "خطای غیرمنتظره‌ای رخ داده است | کد -52",
            -54 => "اتوریتی نامعتبر است | کد -54",
            -60 => "امکان ریورس کردن تراکنش با بانک وجود ندارد | کد -60",
            -63 => "زمان مجاز برای ریورس منقضی شده است | کد -63",
            _ => $"خطای نامشخص | کد {dataCode} ",
        };

    }

    private static bool InternalVerify(VerifyRequest request, VerifyResult result, CallBackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.StatusDescription = "Call Back is empty";
            return false;
        }

        if (callbackData.Authority != request.PaymentInfo.CreateReference)
        {
            result.StatusDescription = "مغایرت در مقدار Authority";
            return false;
        }

        if (callbackData.Status != "OK")
        {
            result.StatusDescription = "وضعیت نامعتبر";
            return false;
        }

        return true;
    }

    #endregion

}
