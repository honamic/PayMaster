using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.ReceiptIssuers;
using Honamic.PayMaster.Core.ReceiptRequests.Parameters;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.ReceiptRequests;

public class ReceiptRequest : AggregateRoot<long>
{
    public ReceiptRequest()
    {
        GatewayPayments = [];
    }

    public ReceiptRequestStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; set; } = default!;

    public string? Description { get; set; }

    public string? AdditionalData { get; set; }
    public string? Mobile { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public bool? IsLegal { get; private set; }
    public string? PartyIdentity { get; private set; }
    public long? PartyId { get; private set; }

    public ReceiptIssuer Issuer { get; set; }
    public long IssuerId { get; set; }

    public List<ReceiptRequestGatewayPayment> GatewayPayments { get; set; }

    public static ReceiptRequest Create(CreateReceiptRequestParameters createParameters,
        IIdGenerator idGenerator)
    {
        if (createParameters.Amount <= 0)
        {
            throw new ArgumentException($"مبلغ نمی تواند صفر یا کوچکتر باشد.");
        }

        if (!createParameters.SupportedCurrencies.Contains(createParameters.Currency))
        {
            throw new ArgumentException($"واحد پولی {createParameters.Currency} مجاز نیست.");
        }

        if (!createParameters.Issuer.Enabled)
        {
            throw new ArgumentException($"صادرکننده فیش غیرفعال است.");
        }

        var newReceiptRequest = new ReceiptRequest()
        {
            Id = idGenerator.GetNewId(),
            Status = ReceiptRequestStatus.New,
            Amount = createParameters.Amount,
            Currency = createParameters.Currency,

            AdditionalData = createParameters.AdditionalData,
            Email = createParameters.Email,
            Description = createParameters.Description,
            IsLegal = createParameters.IsLegal,
            Mobile = createParameters.Mobile,

            PartyId = createParameters.PartyId,
            PartyIdentity = createParameters.PartyIdentity,
            NationalityCode = createParameters.NationalityCode,
            IssuerId = createParameters.Issuer.Id,
        };

        CreateGatewayPaymentParameters createGatewayPayment = new CreateGatewayPaymentParameters
        {
            Id = idGenerator.GetNewId(),
            Amount = createParameters.Amount,
            Currency = createParameters.Currency,
            GatewayProvider = createParameters.GatewayProvider,
        };

        ReceiptRequestGatewayPayment newPayment = ReceiptRequestGatewayPayment.Create(createGatewayPayment);

        newReceiptRequest.GatewayPayments.Add(newPayment);

        return newReceiptRequest;
    }
}