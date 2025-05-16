using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.ReceiptIssuers;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.ReceiptRequests;

public class ReceiptRequest : AggregateRoot<long>
{
    public ReceiptRequest()
    {
        GatewayPayments = [];
    }

    public ReceiptRequestStatus Status { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; set; } = default!;

    public string? Description { get; set; }

    public string? AdditionalData { get;  set; }
    public string? MobileNumber { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public bool? IsLegal { get; private set; }
    public string? PartyIdentity { get; private set; }
    public long? PartyId { get; private set; }

    public ReceiptIssuer? Issuer { get; set; }
    public long? IssuerId { get; set; }

    public List<ReceiptRequestGatewayPayment> GatewayPayments { get; set; }
}
