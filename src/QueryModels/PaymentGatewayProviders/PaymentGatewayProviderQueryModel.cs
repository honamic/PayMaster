using Honamic.Framework.Queries;

namespace Honamic.PayMaster.QueryModels.PaymentGatewayProviders;
public class PaymentGatewayProviderQueryModel  : AggregateQueryBase<long>
{
    
    public string Title { get; set; }

    public string Code { get; set; }

    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    public string JsonConfigurations { get; set; }

    public bool Enabled { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }
}