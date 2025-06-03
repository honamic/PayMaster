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
public class PayPalPaymentProvider : PaymentGatewayProviderBase<PayPalConfigurations>
{
    private readonly ILogger<PayPalPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;


    public PayPalPaymentProvider(ILogger<PayPalPaymentProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
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
                     Amount= new PayPalMoney
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
                            BrandName = "Iman",
                        }
                    }
                }
            };

            HttpRequestMessage httpRequest = CreateHttpRequest(apiRequest);

            result.LogData.Start(apiRequest, httpRequest.RequestUri?.ToString());

            HttpResponseMessage apiResponse = await client.SendAsync(httpRequest);

            result.LogData.End();

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.SetResponse(rawResponse);

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

    public override async Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerifyResult();

        try
        {
            var callbackData = (PayPalCallBackDataModel?)request.CallBackData;

            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                return result;
            }

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var orderId = callbackData?.Token ?? "";


            //------------------- Get Order -----------
            HttpRequestMessage httpRequestGetOrder = CreateGetOrderHttpRequest(orderId);

            result.VerifyLogData.Start(orderId, httpRequestGetOrder.RequestUri?.ToString());

            var getOrderResponse = await client.SendAsync(httpRequestGetOrder);

            result.VerifyLogData.End();

            var getOrderResponseString = await getOrderResponse.Content.ReadAsStringAsync();

            result.VerifyLogData.SetResponse(getOrderResponseString);

            if (getOrderResponse.IsSuccessStatusCode)
            {
                var payPalOrder = JsonSerializer.Deserialize<PayPalOrder>(getOrderResponseString);

                if (payPalOrder?.Status != PayPalOrderStatus.Approved)
                {
                    result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                    result.StatusDescription = $"Status not Valid {payPalOrder?.Status}";
                    return result;
                }

                var amount = decimal.Parse(payPalOrder?.PurchaseUnits[0].Amount?.Value ?? "0");
                if (amount != request.PaymentInfo.Amount)
                {
                    result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                    result.StatusDescription = $"Amount not Valid [{amount}]";
                    return result;
                }
            }

            //------------------- Capture Order -----------

            HttpRequestMessage httpRequest = CreateCaptureHttpRequest(orderId);

            result.StartSettlement();
            result.SettlementLogData!.Start(orderId, httpRequest.RequestUri?.ToString());

            var verifyResponse = await client.SendAsync(httpRequest);

            result.SettlementLogData.End();

            var verifyResponseString = await verifyResponse.Content.ReadAsStringAsync();

            result.SettlementLogData.SetResponse(verifyResponseString);

            if (!verifyResponse.IsSuccessStatusCode)
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                result.StatusDescription = verifyResponse.StatusCode.ToString();
                return result;
            }

            var payPalCaptureOrder = JsonSerializer.Deserialize<PayPalOrder>(verifyResponseString);

            if (payPalCaptureOrder?.PurchaseUnits?[0]?.Payments?.Captures?[0].Id != null)
            {
                result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
                {
                    SuccessReference = payPalCaptureOrder?.PurchaseUnits?[0]?.Payments?.Captures?[0].Id,
                };
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.StatusDescription = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    private HttpRequestMessage CreateHttpRequest(PaypalCreateOrder apiRequest)
    {
        string apiRequestJsonData = JsonSerializer.Serialize(apiRequest);

        HttpRequestMessage httpRequest = new(HttpMethod.Post, Configurations.CreateOrderUrl())
        {
            Content = new StringContent(apiRequestJsonData, Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, Configurations);

        return httpRequest;
    }

    private HttpRequestMessage CreateCaptureHttpRequest(string orderId)
    {
        HttpRequestMessage httpRequest = new(HttpMethod.Post, Configurations.CaptureUrl(orderId))
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, Configurations);

        return httpRequest;
    }

    private HttpRequestMessage CreateGetOrderHttpRequest(string orderId)
    {
        HttpRequestMessage httpRequest = new(HttpMethod.Get, Configurations.GetOrderUrl(orderId))
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        var customOptionKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        httpRequest.Options.Set(customOptionKey, Configurations);

        return httpRequest;
    }

    private static bool InternalVerify(VerifyRequest request, VerifyResult result, PayPalCallBackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.StatusDescription = "Call Back is empty";
            return false;
        }

        if (callbackData.Token != request.PaymentInfo.CreateReference)
        {
            result.StatusDescription = "مغایرت در OrderId";
            return false;
        }

        return true;
    }
}
