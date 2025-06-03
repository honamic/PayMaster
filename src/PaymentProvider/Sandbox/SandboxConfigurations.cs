using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Sandbox;

public class SandboxConfigurations : IPaymentGatewayProviderConfiguration
{
    public SandboxConfigurations()
    {
        SetDefaultConfiguration();
    }

    public string PayUrl { get; set; }

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        PayUrl = "https://yoursite.com/paymaster/sandbox/pay";
    }

    public List<string> IsValid()
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
