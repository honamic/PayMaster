using Honamic.PayMaster.PaymentProvider.Behpardakht.Dtos;
using Honamic.PayMaster.PaymentProvider.Core;
using Honamic.PayMaster.PaymentProvider.Core.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.Behpardakht;
public class BehpardakhtPaymentProvider : PaymentProviderBase
{
    private const string ParamsForGatewayPath = "/pgwchannel/services/pgw";
    private readonly ILogger<BehpardakhtPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private BehpardakhtConfigurations Configurations = new BehpardakhtConfigurations();

    public BehpardakhtPaymentProvider(ILogger<BehpardakhtPaymentProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public override void Configure(string jsonConfiguration)
    {
        var options = JsonSerializer.Deserialize<BehpardakhtConfigurations>(jsonConfiguration);

        if (options == null)
        {
            throw new ArgumentNullException(nameof(jsonConfiguration));
        }

        Configurations = options;
    }

    public override async Task<ParamsForPayResult> ParamsForPayAsync(ParamsForPayRequest request)
    {
        var result = new ParamsForPayResult();

        try
        {
            var url = new Uri(new Uri(Configurations.ApiAddress), ParamsForGatewayPath);

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var apiRequest = new MellatPrepareRequest
            {
                MerchantId = Configurations.TerminalId,
                Amount = request.Amount,
                CallbackUrl = request.CallbackUrl,
                OrderId = request.UniqueRequestId,
            };

            result.LogData.Request = apiRequest;

            var apiResponse = await client.PostAsJsonAsync(url, apiRequest);

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.Response = rawResponse;

            if (apiResponse.IsSuccessStatusCode)
            {
                var mellatResponse = JsonSerializer.Deserialize<MellatPrepareResponse>(rawResponse);
                result.PayUrl = Configurations.PayUrl;
                result.PayVerb = PayVerb.Post;
                result.PayParams.Add("RefId", mellatResponse?.RefId ?? "null");
                result.Success = true;
            }
            else
            {

            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            _logger.LogError(ex, "PrePay Failed");
        }

        return result;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<CallBackDataModel>(callBackJsonValue);

            if (!string.IsNullOrEmpty(callbackData?.SaleOrderId))
            {
                result.UniqueRequestId = callbackData.SaleReferenceId;
                result.Token = callbackData.RefId;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else
            {
                result.Error = "SaleOrderId value not found.";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }
}
