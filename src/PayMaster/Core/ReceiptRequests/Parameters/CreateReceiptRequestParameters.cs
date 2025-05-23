namespace Honamic.PayMaster.Core.ReceiptRequests.Parameters;
public class CreateReceiptRequestParameters
{
    public required decimal Amount { get; set; }
    public required string Currency { get; set; } = default!;

    public string? Description { get; set; }

    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? NationalityCode { get; set; }
    public string? PartyReference { get; set; }
    public string? IssuerReference { get; set; }
    public long? PartyId { get; set; }
    public bool? IsLegal { get; set; }

    public string? AdditionalData { get; set; }

    public required ReceiptRequestIssuerParameters Issuer { get; set; }
    public required ReceiptRequestGatewayProviderParameters GatewayProvider { get; set; }

    public required string[] SupportedCurrencies;
}

public class ReceiptRequestIssuerParameters
{
    public long Id { get; internal set; }
    public bool Enabled { get; internal set; }
}

public class ReceiptRequestGatewayProviderParameters
{
    public long Id { get; internal set; }
    public bool Enabled { get; internal set; }
    public decimal? MinimumAmount { get; internal set; }
    public decimal? MaximumAmount { get; internal set; }
}