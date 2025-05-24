using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.PaymentGatewayProviders;
public class PaymentGatewayProvider : AggregateRoot<long>
{
    public PaymentGatewayProvider()
    {
        Configurations = "{}";
        ProviderType = "";
    }

    public string Title { get; set; }

    public string Code { get; set; }

    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    /// <summary>
    /// Json configuration
    /// </summary>
    public string Configurations { get; set; }

    public bool Enabled { get; set; }

    public int Order { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }
}