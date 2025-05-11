using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.PaymentProviders.Entities;

public class PaymentRequest
{
    public long Id { get; private set; }
    public Money Money { get; private set; }
    public Card? Card { get; private set; }
    public Payer? Payer { get; private set; }
    public string CallbackUrl { get; private set; }
    public long UniqueRequestId { get; private set; }
    public string? MobileNumber { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public string? GatewayNote { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public PaymentFailedReason? PaymentFailedReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private PaymentRequest(Money money, string callbackUrl, long uniqueRequestId)
    {
        Money = money;
        CallbackUrl = callbackUrl;
        UniqueRequestId = uniqueRequestId;
        PaymentStatus = PaymentStatus.New;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public static PaymentRequest Create(Money money, string callbackUrl, long uniqueRequestId)
    {
        if (money == null)
            throw new ArgumentNullException(nameof(money), "Money is required.");

        if (string.IsNullOrWhiteSpace(callbackUrl))
            throw new ArgumentException("Callback URL is required.", nameof(callbackUrl));

        if (uniqueRequestId <= 0)
            throw new ArgumentException("Unique Request ID must be a positive number.", nameof(uniqueRequestId));

        return new PaymentRequest(money, callbackUrl, uniqueRequestId);
    }

    public void SetCard(Card card)
    {
        Card = card ?? throw new ArgumentNullException(nameof(card), "Card is required.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPayer(Payer payer)
    {
        Payer = payer ?? throw new ArgumentNullException(nameof(payer), "Payer is required.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePaymentStatus(PaymentStatus status, PaymentFailedReason? failedReason = null)
    {
        if (status == PaymentStatus.Failed && failedReason == null)
            throw new InvalidOperationException("Failed reason must be provided when status is Failed.");

        PaymentStatus = status;
        PaymentFailedReason = failedReason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMobileNumber(string? mobileNumber)
    {
        if (!string.IsNullOrEmpty(mobileNumber) && mobileNumber.Length != 11)
            throw new ArgumentException("Mobile number must be 11 digits.", nameof(mobileNumber));

        MobileNumber = mobileNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNationalityCode(string? nationalityCode)
    {
        if (!string.IsNullOrEmpty(nationalityCode) && nationalityCode.Length != 10)
            throw new ArgumentException("Nationality code must be 10 digits.", nameof(nationalityCode));

        NationalityCode = nationalityCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmail(string? email)
    {
        if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            throw new ArgumentException("Invalid email format.", nameof(email));

        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetGatewayNote(string? gatewayNote)
    {
        if (!string.IsNullOrEmpty(gatewayNote) && gatewayNote.Length > 500)
            throw new ArgumentException("Gateway note cannot exceed 500 characters.", nameof(gatewayNote));

        GatewayNote = gatewayNote;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Validate()
    {
        if (Money == null)
            throw new InvalidOperationException("Money is required.");

        if (string.IsNullOrWhiteSpace(CallbackUrl))
            throw new InvalidOperationException("Callback URL is required.");

        if (UniqueRequestId <= 0)
            throw new InvalidOperationException("Unique Request ID must be a positive number.");
    }
}
