namespace Honamic.PayMaster.PaymentProvider.Sandbox;

public class SanboxConfigurations
{
    public SanboxConfigurations()
    {
        PayUrl = "https://yoursite.com//paymaster/sandbox/pay";
    }

    public string PayUrl { get;  set; }
}
