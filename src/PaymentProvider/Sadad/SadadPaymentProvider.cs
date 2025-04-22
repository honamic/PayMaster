using Honamic.PayMaster.PaymentProvider.Sadad.Models;
using Honamic.PayMaster.PaymentProvider.Core;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace Honamic.PayMaster.PaymentProvider.Sadad;
public class SadadPaymentProvider : PaymentProviderBase
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

    public override async Task<ParamsForPayResult> ParamsForPayAsync(ParamsForPayRequest request)
    {
        var result = new ParamsForPayResult();

        try
        {
            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var apiRequest = new PaymentRequest
            {
                Amount = request.Amount,
                OrderId = request.UniqueRequestId,
                LocalDateTime = DateTime.Now,
                TerminalId = Configurations.TerminalId,
                MerchantId = Configurations.MerchantId,
                ReturnUrl = request.CallbackUrl,
                SignData = CreateSign(request, Configurations),
            };

            result.LogData.Request = apiRequest;

            var apiResponse = await client.PostAsJsonAsync(Configurations.PaymentRequestUri, apiRequest);

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.Response = rawResponse;

            if (apiResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<PaymentResponse>(rawResponse);

                if (response?.ResCode == "0")
                {
                    //.../Purchase/Index?token={1}
                    result.PayUrl = Configurations.PurchasePage;
                    result.PayVerb = PayVerb.Get;
                    result.PayParams.Add("Token", response?.Token ?? "null");
                    result.Success = true;
                }
                else
                {
                    result.Error = response?.Description;
                }
            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            result.Error = ex.Message;
            _logger.LogError(ex, "PrePay Failed");
        }

        return result;
    }

    private static string CreateSign(ParamsForPayRequest request, SadadConfigurations configurations)
    {
        var data = $"{configurations.TerminalId};{request.UniqueRequestId};{request.Amount}";

        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var tripleDes = TripleDES.Create();
        tripleDes.Mode = CipherMode.ECB;
        tripleDes.Padding = PaddingMode.PKCS7;

        var encryptor = tripleDes.CreateEncryptor(
            Convert.FromBase64String(configurations.MerchantKey), new byte[8]);

        var bytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

        return Convert.ToBase64String(bytes);
    }
}
