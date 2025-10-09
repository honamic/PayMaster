using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.PayPal;

public class PayPalConfigurations : IPaymentGatewayProviderConfiguration
{
    public PayPalConfigurations()
    {

    }


    public string ApiAddress { get; set; } = default!;

    public string PayUrl { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string Secret { get; set; } = default!;

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        ApiAddress = sandbox ? "https://api-m.sandbox.paypal.com" : "https://api-m.paypal.com";
        PayUrl = sandbox ? "https://www.sandbox.paypal.com" : "https://www.paypal.com";
        ClientId = "YOUR_CLIENT_ID";
        Secret = "YOUR_SECRET";
    }

    public List<string> GetValidationErrors()
    {
        List<string> errors = new List<string>();

        if (string.IsNullOrEmpty(this.ClientId))
        {
            errors.Add("ClientId must be set.");
        }

        if (string.IsNullOrEmpty(this.Secret))
        {
            errors.Add("Secret must be set.");
        }

        if (string.IsNullOrEmpty(this.ApiAddress))
        {
            errors.Add("ApiAddress must be set.");
        }
        else if (!Uri.IsWellFormedUriString(this.ApiAddress, UriKind.Absolute))
        {
            errors.Add("ApiAddress is not a valid URL.");
        }

        if (string.IsNullOrEmpty(this.PayUrl))
        {
            errors.Add("PayUrl must be set.");
        }
        else if (!Uri.IsWellFormedUriString(this.PayUrl, UriKind.Absolute))
        {
            errors.Add("PayUrl is not a valid URL.");
        }

        return errors;
    }

    public Uri CreateOrderUrl()
    {
        return PayMaster.Extensions.UriExtensions.Combine(ApiAddress, Constants.CheckoutOrdersPath);
    }

    public Uri CaptureUrl(string orderId)
    {
        var path = PayMaster.Extensions.UriExtensions.JoinUrlSegments(Constants.CheckoutOrdersPath, $"/{orderId}/capture");
        return PayMaster.Extensions.UriExtensions.Combine(ApiAddress, path);
    }

    public Uri GetOrderUrl(string orderId)
    {
        var path = PayMaster.Extensions.UriExtensions.JoinUrlSegments(Constants.CheckoutOrdersPath, orderId);
        return PayMaster.Extensions.UriExtensions.Combine(ApiAddress, path);
    }
}
