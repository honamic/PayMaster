using Honamic.PayMaster.PaymentProvider.DigiPay.Models;
using Honamic.PayMaster.PaymentProvider.DigiPay.Models.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
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
        throw new NotImplementedException();
    }

    public override Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        throw new NotImplementedException();
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