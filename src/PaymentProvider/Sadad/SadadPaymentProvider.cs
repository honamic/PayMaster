using Honamic.PayMaster.PaymentProvider.Sadad.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using Honamic.PayMaster.PaymentProvider.Sadad.Extensions;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.PaymentProviders;
using System.Net.Http;

namespace Honamic.PayMaster.PaymentProvider.Sadad;
public class SadadPaymentProvider : PaymentGatewayProviderBase
{
    private const string PaymentRequestPath = "/api/v0/Request/PaymentRequest";
    private readonly ILogger<SadadPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private SadadConfigurations Configurations = new SadadConfigurations();

    public SadadPaymentProvider(ILogger<SadadPaymentProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public override void Configure(string jsonConfiguration)
    {
        var options = JsonSerializer.Deserialize<SadadConfigurations>(jsonConfiguration);

        if (options == null)
        {
            throw new ArgumentNullException(nameof(jsonConfiguration));
        }

        Configurations = options;
    }

    public override async Task<CreateResult> CreateAsync(CreateRequest request)
    {
        var result = new CreateResult();

        try
        {
            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var apiRequest = new PaymentRequest
            {
                Amount = request.Amount,
                OrderId = request.UniqueRequestId.ToString(),
                LocalDateTime = DateTime.Now,
                TerminalId = Configurations.TerminalId,
                MerchantId = Configurations.MerchantId,
                ReturnUrl = request.CallbackUrl,
                SignData = SadadPaymentProviderHelpers.CreateSign(request, Configurations),
            };

            result.LogData.Start(apiRequest, Configurations.PaymentRequestUri);

            var apiResponse = await client.PostAsJsonAsync(Configurations.PaymentRequestUri, apiRequest);

            result.LogData.End();

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.SetResponse(rawResponse);

            if (apiResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<PaymentResponse>(rawResponse);

                if (response?.ResCode == "0")
                {
                    //.../Purchase/Index?token={1}
                    result.PayUrl = Configurations.PurchasePage;
                    result.PayVerb = PayVerb.Get;
                    result.PayParams.Add("Token", response?.Token ?? "null");
                    result.CreateReference = response?.Token;
                    result.Success = true;
                }
                else
                {
                    result.StatusDescription = response?.Description;
                }
            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            result.StatusDescription = ex.Message;
            _logger.LogError(ex, "PrePay Failed");
        }

        return result;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<CallBackData>(callBackJsonValue);

            if (!string.IsNullOrEmpty(callbackData?.OrderId))
            {
                result.UniqueRequestId = long.Parse(callbackData.OrderId ?? "-1");
                result.CreateReference = callbackData.Token;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else
            {
                result.Error = "OrderId Value not found.";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        throw new NotImplementedException();
    }
}
