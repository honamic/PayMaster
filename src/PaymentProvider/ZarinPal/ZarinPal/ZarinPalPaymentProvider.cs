using Honamic.PayMaster.PaymentProvider.Core;
using Honamic.PayMaster.PaymentProvider.Core.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Models;
using System.Text;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class ZarinPalPaymentProvider(
    IHttpClientFactory httpClientFactory,
    ILogger<ZarinPalPaymentProvider> logger)
    : PaymentProviderBase
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

            result.LogData.Request = apiRequest;

            var apiResponse = await client.PostAsJsonAsync(url, apiRequest);

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.Response = rawResponse;

            if (apiResponse.IsSuccessStatusCode)
            {
                var zarinPalResult = JsonSerializer.Deserialize<ZarinPalResult<PaymentRequestResponse>>(rawResponse);
                if (zarinPalResult is not { data.code: 100 })
                {
                    result.Error = GetDescriptionFromCode(zarinPalResult?.data.code);
                    return result;
                }

                var payUrl = $"{_configurations.PayUrl.TrimEnd('/')}/{zarinPalResult.data.authority}";

                result.PayUrl = payUrl;
                result.PayVerb = PayVerb.Get;
                result.CreateToken = zarinPalResult.data.authority;
                result.Success = true;
                return result;
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
                //زرین پال توی callback فقط یک status میده و یک authority که شناسه ای هست که باهاش رفتیم درگاه
                //result.UniqueRequestId = ;
                result.CreateToken = callbackData.Authority;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else
            {
                result.Error = "Status is not OK!";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override async Task<VerfiyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerfiyResult();
        try
        {
            var callbackData = (CallBackDataModel?)request.CallBackData;
            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentFailedReason.InternalVerfiy;
                return result;
            }

            var verificationRequest = new PaymentVerificationRequest
            {
                merchant_id = _configurations.MerchantId,
                amount = request.PatmentInfo.Amount,
                authority = callbackData!.Authority
            };
            result.LogData.Request = verificationRequest;

            var json = JsonSerializer.Serialize(verificationRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = httpClientFactory.CreateClient(Constants.HttpClientName);
            var url = new Uri(new Uri(_configurations.ApiAddress), Constants.PAYMENT_VERIFICATION_URL);
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            result.LogData.Response = responseString;

            var paymentVerificationResponse = JsonSerializer.Deserialize<ZarinPalResult<PaymentVerificationResponse>>(responseString);

            if (paymentVerificationResponse is null || paymentVerificationResponse.data.code != 100)
            {
                result.PaymentFailedReason = PaymentFailedReason.Verfiy;
                result.Error = GetDescriptionFromCode(paymentVerificationResponse?.data.code);
                return result;
            }

            result.Success = true;
            result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
            {
                MerchantId = _configurations.MerchantId,
                Pan = paymentVerificationResponse.data.card_pan,
                ReferenceRetrievalNumber = $"",
                RefNum = $"{paymentVerificationResponse.data.ref_id}",
                TerminalId = $"{_configurations.TerminalId}",
                TrackingNumber = paymentVerificationResponse.data.card_hash
            };
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
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
            null => "خطای نامشخص در پردازش درخواست",
            100 => "عملیات موفق",
            101 => "عملیات قبلا انجام شده است",
            _ => "عملیات ناموفق",
        };
    }

    private static bool InternalVerify(VerifyRequest request, VerfiyResult result, CallBackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.Error = "Call Back is empty";
            return false;
        }

        if (callbackData.Authority != request.PatmentInfo.CreateToken)
        {
            result.Error = "مغایرت در توکن";
            return false;
        }

        if (callbackData.Status != "OK")
        {
            result.Error = "وضعیت نامعتبر";
            return false;
        }

        return true;
    }

    #endregion

}
