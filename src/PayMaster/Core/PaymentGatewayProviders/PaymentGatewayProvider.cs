using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Core.PaymentGatewayProviders;
public class PaymentGatewayProvider: AggregateRoot<long>
{
    public PaymentGatewayProvider()
    {
        Configurations = "{}";
        ProviderType = "";
    }

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