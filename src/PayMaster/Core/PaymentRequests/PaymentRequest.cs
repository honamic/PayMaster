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

    public decimal Amount { get; private set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; private set; } = default!;

    public string? Description { get; private set; }

    public string? AdditionalData { get; private set; }
    public string? MobileNumber { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public bool? IsLegal { get; private set; }
    public string? PartyIdentity { get; private set; }
    public long? PartyId { get; private set; }

    public PaymentRequester? Requester { get; private set; }
    public long? RequesterRef { get; private set; }


    public List<PaymentRequestPaymentGateway> GatewayPayments { get; set; }
}
