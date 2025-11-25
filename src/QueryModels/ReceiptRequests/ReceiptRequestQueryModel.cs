using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.ReceiptRequests;

public class ReceiptRequestQueryModel : AggregateRootQueryBase<long>
{
    
    public ReceiptRequestStatus Status { get;  set; }

    public decimal Amount { get;  set; }
 
    public string Currency { get;  set; } = default!;

    public long IssuerId { get;  set; }

    public ReceiptIssuerQueryModel Issuer { get; set; } = default!;

    public string? Description { get;  set; }

    public string? AdditionalData { get;  set; }

    public string? Mobile { get;  set; }
    
    public string? NationalityCode { get;  set; }
    
    public string? Email { get;  set; }
    
    public bool? IsLegal { get;  set; }
    
    public string? IssuerReference { get;  set; }
    
    public string? PartyReference { get;  set; }

    public long? PartyId { get;  set; }


    public List<ReceiptRequestGatewayPaymentQueryModel> GatewayPayments { get; set; }

    public List<ReceiptRequestTryLogQueryModel> TryLogs { get; set; }
    
}