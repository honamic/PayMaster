using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.PaymentGatewayProviders;
public class PaymentGatewayProvider : AggregateRoot<long>
{
    public PaymentGatewayProvider()
    {
        JsonConfigurations = "{}";
        ProviderType = "";
    }

    public string Title { get; set; }

    public string Code { get; set; }

    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    public string JsonConfigurations { get; set; }

    public bool Enabled { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }
}