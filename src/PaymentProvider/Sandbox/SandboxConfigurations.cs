using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Sandbox;

public class SandboxConfigurations : IPaymentGatewayProviderConfiguration
{
    public SandboxConfigurations()
    {

    }

    public string PayUrl { get; set; } = default!;

    public string MerchantName { get; set; } = default!;

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        PayUrl = "https://yoursite.com/paymaster/sandbox/pay";
        MerchantName = "نام پذیرنده محیط تست";
    }

    public List<string> GetValidationErrors()
    {
        if (string.IsNullOrEmpty(PayUrl))
        {
            return ["PayUrl is required"];
        }
        else if (!Uri.IsWellFormedUriString(PayUrl, UriKind.Absolute))
        {
            return ["PayUrl is not a valid URL"];
        }

        return [];
    }
}
