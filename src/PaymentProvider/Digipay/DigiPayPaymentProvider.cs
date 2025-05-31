using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProvider.DigiPay.Models;
using Honamic.PayMaster.PaymentProvider.DigiPay.Models.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.DigiPay;

public class DigiPayPaymentProvider : PaymentGatewayProviderBase
{
    private readonly ILogger<DigiPayPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private DigipayConfigurations _configurations = new DigipayConfigurations();

    public DigiPayPaymentProvider(IHttpClientFactory httpClientFactory, ILogger<DigiPayPaymentProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }


    public override void Configure(string providerConfiguration)
    {
        var zarinPalConfigurations = JsonSerializer.Deserialize<DigipayConfigurations>(providerConfiguration);

        _configurations = zarinPalConfigurations ??
                         throw new ArgumentNullException(nameof(providerConfiguration));
    }

    public override async Task<CreateResult> CreateAsync(CreateRequest createRequest)
    {
        var result = new CreateResult();

        try
        {
            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var digipayRequest = new TicketRequestDto
            {
                ProviderId = createRequest.UniqueRequestId.ToString(),
                Amount = createRequest.Amount,
                CallbackUrl = createRequest.CallbackUrl,
                CellNumber = createRequest.CallbackUrl,
            };

            var httpRequest = CreateHttpRequest(digipayRequest, createRequest);

            result.LogData.Start(digipayRequest, httpRequest.RequestUri?.ToString());

            HttpResponseMessage apiResponse = await client.SendAsync(httpRequest);

            result.LogData.End();

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.SetResponse(rawResponse);

            if (apiResponse.IsSuccessStatusCode)
            {
                var ticketResponse = JsonSerializer.Deserialize<TicketResponseDto>(rawResponse);
                if (ticketResponse is not { Result.Status: 0 })
                {
                    result.Error = GetDescriptionWithCode(ticketResponse?.Result?.Status);
                    return result;
                }

                result.PayUrl = ticketResponse.RedirectUrl;
                result.PayVerb = PayVerb.Post;
                result.CreateReference = ticketResponse.Ticket;
                result.Success = true;
                return result;

            }
            else
            {
                var ticketfaieldResponse = JsonSerializer.Deserialize<TicketResponseDto?>(rawResponse);
                var errorCode = ticketfaieldResponse?.Result.Status;

                if (errorCode != null)
                {
                    result.Error = GetDescriptionWithCode(errorCode);
                }
                else
                {
                    result.Error = $"Status Code:{apiResponse.StatusCode}";
                }
            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            _logger.LogError(ex, "digipay create failed.");
        }

        return result;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<DigipayCallbackDataModel>(callBackJsonValue);

            if (callbackData is { Result: "SUCCESS" })
            {
                result.UniqueRequestId = long.Parse(callbackData.ProviderId);
                result.CreateReference = null;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else if (callbackData is { Result: "CANCEL" })
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Canceled;
            }
            else
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Other;
                result.Error = $"Status is not valid! [{callbackData?.Result}]";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override async Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerifyResult();

        try
        {
            var callbackData = (DigipayCallbackDataModel?)request.CallBackData;

            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                return result;
            }

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var trackingCode = callbackData?.TrackingCode ?? "";


            HttpRequestMessage httpRequestGetOrder = CreateVerifyHttpRequest(trackingCode, callbackData.Type);

            result.VerifyLogData.Start(trackingCode, httpRequestGetOrder.RequestUri?.ToString());

            var verifyResponse = await client.SendAsync(httpRequestGetOrder);

            result.VerifyLogData.End();

            var verifyResponseString = await verifyResponse.Content.ReadAsStringAsync();

            result.VerifyLogData.SetResponse(verifyResponseString);

            if (verifyResponse.IsSuccessStatusCode)
            {
                var digipayVerify = JsonSerializer.Deserialize<DigipayVerifyModel>(verifyResponseString);

                if (callbackData is not { Result: "SUCCESS" })
                {
                    result.PaymentFailedReason = PaymentGatewayFailedReason.Verfiy;
                    result.Error = digipayVerify?.Result?.Message?.Trim();
                    if (string.IsNullOrEmpty(result.Error))
                    {
                        result.Error = $"Status not Valid {digipayVerify?.Result.Status}";
                    }
                    return result;
                }

                if (digipayVerify?.Amount != request.PatmentInfo.Amount)
                {
                    result.PaymentFailedReason = PaymentGatewayFailedReason.Verfiy;
                    result.Error = $"Amount not Valid [{digipayVerify?.Amount}]";
                    return result;
                }

                result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
                {
                    SuccessReference = digipayVerify.TrackingCode,
                    MerchantId = null,
                    TerminalId = digipayVerify.TerminalId,
                    Pan = digipayVerify.MaskedPan,
                    ReferenceRetrievalNumber = digipayVerify.Rrn,
                    TrackingNumber = null,
                };

                result.Success = true;
            }
            else
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Verfiy;
                result.Error = verifyResponse.StatusCode.ToString();
                return result;
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    private bool InternalVerify(VerifyRequest request, VerifyResult result, DigipayCallbackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.Error = "Call Back is empty";
            return false;
        }

        return true;
    }

    private HttpRequestMessage CreateVerifyHttpRequest(string trackingCode, TicketType type)
    {
        var verfiyPath = Constants.CreatePath + $"/{trackingCode}/?type={(int)type}";
        var url = new Uri(new Uri(_configurations.ApiAddress), verfiyPath);

        HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<DigipayConfigurations>(Constants.DigiPayRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, _configurations);

        return httpRequest;
    }

    private HttpRequestMessage CreateHttpRequest(TicketRequestDto apiRequest, CreateRequest createRequest)
    {
        var createPath = Constants.CreatePath + $"?type={MapTicketType(createRequest)}";
        var url = new Uri(new Uri(_configurations.ApiAddress), createPath);

        string apiRequestJsonData = JsonSerializer.Serialize(apiRequest);

        HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = new StringContent(apiRequestJsonData, Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<DigipayConfigurations>(Constants.DigiPayRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, _configurations);

        return httpRequest;
    }

    private string GetDescriptionWithCode(int? status)
    {
        return $"کد {status} | {GetDescriptionFromCode(status)}";
    }

    private int MapTicketType(CreateRequest createRequest)
    {
        return (int)TicketType.WALLET;
    }

    private string GetDescriptionFromCode(int? status)
    {
        return status switch
        {
            0 => "عملیات با موفقیت انجام شد",
            1054 => "اطلاعات ورودی اشتباه می باشد",
            9000 => "اطلاعات خرید یافت نشد",
            9001 => "توکن پرداخت معتبر نمی باشد",
            9003 => "خرید مورد نظر منقضی شده است",
            9004 => "خرید مورد نظر درحال انجام است",
            9005 => "خرید قابل پرداخت نمی باشد",
            9006 => "خطا در برقراری ارتباط با درگاه پرداخت",
            9007 => "خرید با موفقیت انجام نشده است",
            9008 => "این خرید با داده های متفاوتی قبلا ثبت شده است",
            9009 => "محدوده زمانی تایید تراکنش گذشته است",
            9010 => "تایید خرید ناموفق بود",
            9011 => "نتیجه تایید خرید نامشخص است",
            9012 => "وضعیت خرید برای این درخواست صحیح نمی باشد",
            9030 => "ورود شماره همراه برای کاربران ثبت نام شده الزامی است",
            9031 => "اعطای تیکت برای کاربر مورد نظر امکان پذیر نمی‌باشد",
            _ => "کد وضعیت تعریف نشده"
        };
    }
}