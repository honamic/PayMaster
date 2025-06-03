using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class ZarinPalConfigurations : IPaymentGatewayProviderConfiguration
{
    public ZarinPalConfigurations()
    {
        SetDefaultConfiguration();
    }

    public string ApiAddress { get; set; }

    public string PayUrl { get; set; }

    public string MerchantId { get; set; }

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        MerchantId = "YourMerchantId";

        ApiAddress = sandbox
            ? "https://sandbox.zarinpal.com/"
            : "https://api.zarinpal.com/";

        PayUrl = sandbox
            ? "https://sandbox.zarinpal.com/pg/StartPay/"
            : "https://www.zarinpal.com/pg/StartPay/";
    }

    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(MerchantId))
        {
            errors.Add("MerchantId is required");
        }

        if (string.IsNullOrEmpty(ApiAddress))
        {
            errors.Add("ApiAddress is required");
        }
        else if (!Uri.IsWellFormedUriString(ApiAddress, UriKind.Absolute))
        {
            errors.Add("ApiAddress is not a valid URL");
        }

        if (string.IsNullOrEmpty(PayUrl))
        {
            errors.Add("PayUrl is required");
        }
        else if (!Uri.IsWellFormedUriString(PayUrl, UriKind.Absolute))
        {
            errors.Add("PayUrl is not a valid URL");
        }

        return errors;
    }

    public Uri PaymentRequestUrl()
    {
        return PayMaster.Extensions.UriExtensions.Combine(ApiAddress, Constants.PAYMENT_REQUEST_URL);
    }

    public Uri PaymentVerificationUrl()
    {
        return PayMaster.Extensions.UriExtensions.Combine(ApiAddress, Constants.PAYMENT_VERIFICATION_URL);
    }
}