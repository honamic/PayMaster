namespace Honamic.PayMaster.Core.PaymentGatewayProviders;

public class PaymentGatewayProvider
{
    public PaymentGatewayProvider()
    {
        Configurations = "{}";
        ProviderType = "";
    }

    public long Id { get; private set; }

    /// <summary>
    /// samples: Behpardakht,Sep,Sadad
    /// </summary>
    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    /// <summary>
    /// Json configuration
    /// </summary>
    public string Configurations { get; set; }

    public bool Enable { get; set; }

    public int Order { get; set; }
}