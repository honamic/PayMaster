using Honamic.PayMaster.Core.PaymentRequesters;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.PaymentRequests;

public class PaymentRequest
{
    public PaymentRequest()
    {
        GatewayPayments = [];
    }

    public long Id { get; set; }

    public PaymentRequestStatus Status { get; set; }

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

    public PaymentRequester? Requester { get; set; }
    public long? RequesterRef { get; set; }


    public List<PaymentRequestPaymentGateway> GatewayPayments { get; set; }
}
