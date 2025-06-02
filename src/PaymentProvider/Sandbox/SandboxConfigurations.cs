namespace Honamic.PayMaster.PaymentProvider.Sandbox;

public class SandboxConfigurations
{
    public SandboxConfigurations()
    {
        PayUrl = "https://yoursite.com//paymaster/sandbox/pay";
    }

    public string PayUrl { get;  set; }
}
