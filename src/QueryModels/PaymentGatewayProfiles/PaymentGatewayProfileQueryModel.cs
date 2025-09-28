using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;
public class PaymentGatewayProfileQueryModel : AggregateQueryBase<long>
{
    
    public string Title { get; set; }

    public string Code { get; set; }

    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    public string JsonConfigurations { get; set; }

    public bool Enabled { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public virtual ICollection<ReceiptRequestGatewayPaymentQueryModel> ReceiptRequestGatewayPayments { get; set; }
}