using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.Application.ReceiptRequests.Queries;

public class GetPublicReceiptRequestQueryResult
{
    public long Id { get; set; }

    public ReceiptRequestStatus Status { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;

    public string? Mobile { get; set; }

    public string? NationalityCode { get; set; }

    public string? Email { get; set; }

    public List<GetPublicReceiptRequestGatewayPaymentQueryResult> GatewayPayments { get; set; } = [];
}

public class GetPublicReceiptRequestGatewayPaymentQueryResult
{
    public long Id { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;

    public PaymentGatewayStatus Status { get; set; }

    public DateTimeOffset? RedirectAt { get; set; }

    public string? SuccessReference { get; set; }

    public string? ReferenceRetrievalNumber { get; set; }

    public string? TrackingNumber { get; set; }

    public string? Pan { get; set; }

    public string PaymentGatewayTitle { get; set; } = default!;
}