using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.PayPal;
public class PayPalPaymentProvider : PaymentProviderBase
{
    private const string checkoutOrdersPath = "/v2/checkout/orders";
    private readonly ILogger<PayPalPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private PayPalConfigurations Configurations = new PayPalConfigurations();

    public PayPalPaymentProvider(ILogger<PayPalPaymentProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public override void Configure(string jsonConfiguration)
    {
        var options = JsonSerializer.Deserialize<PayPalConfigurations>(jsonConfiguration);

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

            var apiRequest = new PaypalCreateOrder
            {
                Intent = PayPalCheckoutPaymentIntent.Capture,
                PurchaseUnits = [ new PayPalPurchaseUnit
                {
                     ReferenceId=request.UniqueRequestId.ToString(),
                     Amount= new PayPalAmount
                     {
                         CurrencyCode=request.Currency!,
                         Value=request.Amount.ToString(CultureInfo.InvariantCulture),
                     },
                }],
                PaymentSource = new PaypalCreateOrderRequestPaymentSource
                {
                    Paypal = new PaypalWalletModel
                    {
                        EmailAddress = request.Email,
                        ExperienceContext = new PayPalExperienceContextModel
                        {
                            CancelUrl = request.CallbackUrl,
                            ReturnUrl = request.CallbackUrl,
                            PaymentMethodPreference = PayPalPayeePaymentMethodPreference.ImmediatePaymentRequired,
                            BrandName="Iman",
                        }
                    }
                }
            };

            HttpRequestMessage httpRequest = CreateHttpRequest(apiRequest);

            result.LogData.Request = apiRequest;

            HttpResponseMessage apiResponse = await client.SendAsync(httpRequest);

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.Response = rawResponse;

            if (apiResponse.IsSuccessStatusCode)
            {
                var payPalResponse = JsonSerializer.Deserialize<PaypalCreateOrderResponse>(rawResponse);

                result.CreateReference = payPalResponse!.Id;

                var paylink = payPalResponse.Links?
                    .FirstOrDefault(c => c.Rel.Equals("payer-action", StringComparison.InvariantCulture));
                result.PayUrl = paylink?.Href;
                result.PayVerb = (paylink?.Method.Equals("GET", StringComparison.InvariantCulture) ?? true)
                                ? PayVerb.Get : PayVerb.Post;

                result.Success = true;
            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            _logger.LogError(ex, "Paypal create failed");
        }

        return result;
    }

    private HttpRequestMessage CreateHttpRequest(PaypalCreateOrder apiRequest)
    {
        var url = new Uri(new Uri(Configurations.ApiAddress), checkoutOrdersPath);

        string apiRequestJsonData = JsonSerializer.Serialize(apiRequest);

        HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = new StringContent(apiRequestJsonData, Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, Configurations);

        return httpRequest;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<PayPalCallBackDataModel>(callBackJsonValue);

            if (!string.IsNullOrEmpty(callbackData?.Token))
            {
                result.CreateReference = callbackData.Token;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else
            {
                result.Error = "the token value is empty.";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override async Task<VerfiyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerfiyResult();

        try
        {
            var callbackData = (PayPalCallBackDataModel?)request.CallBackData;

            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentFailedReason.InternalVerfiy;
                return result;
            }

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var orderId = callbackData?.Token ?? "";

            HttpRequestMessage httpRequest = CreateVerifyHttpRequest(orderId);

            result.LogData.Request = httpRequest.RequestUri?.ToString();

            var verifyResponse = await client.SendAsync(httpRequest);

            var verifyResponseString = await verifyResponse.Content.ReadAsStringAsync();

            result.LogData.Response = verifyResponseString;

            if (!verifyResponse.IsSuccessStatusCode)
            {
                result.PaymentFailedReason = PaymentFailedReason.Verfiy;
                result.Error = verifyResponse.StatusCode.ToString();
                return result;
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }


    private HttpRequestMessage CreateVerifyHttpRequest(string orderId)
    {
        var url = new Uri(new Uri(Configurations.ApiAddress), $"/{orderId}/capture");


        HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, Configurations);

        return httpRequest;
    }

    private static bool InternalVerify(VerifyRequest request, VerfiyResult result, PayPalCallBackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.Error = "Call Back is empty";
            return false;
        }

        if (callbackData.Token != request.PatmentInfo.CreateReference)
        {
            result.Error = "مغایرت در OrderId";
            return false;
        }

        return true;
    }

}
