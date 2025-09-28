using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles; 
using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.ReceiptRequests;

public class ReceiptRequestGatewayPaymentQueryModel : EntityQueryBase<long>
{
    public decimal Amount { get;  set; }

    public string Currency { get;  set; } = default!;

    public PaymentGatewayStatus Status { get;  set; }

    public string? StatusDescription { get;  set; }

    public PaymentGatewayFailedReason FailedReason { get;  set; }

    public long PaymentGatewayProfileId { get; set; }
    public PaymentGatewayProfileQueryModel PaymentGatewayProfile  { get;  set; } = default!;

 
    public string? CreateReference { get;  set; }
 
    public DateTimeOffset? RedirectAt { get;  set; }

 
    public DateTimeOffset? CallbackAt { get;  set; }

    public string? CallbackData { get;  set; }

    public string? SuccessReference { get;  set; }

    public string? ReferenceRetrievalNumber { get;  set; }

    public string? TrackingNumber { get;  set; }

    public string? Pan { get;  set; }

    public string? TerminalId { get;  set; }

    public string? MerchantId { get;  set; }

    public long ReceiptRequestId { get; set; }

}