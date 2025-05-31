using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;

public class DigipayVerifyModel
{
    [JsonPropertyName("trackingCode")]
    public string TrackingCode { get; set; }

    [JsonPropertyName("providerId")]
    public string ProviderId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("paymentGateway")]
    public PaymentGatewayType PaymentGateway { get; set; }

    [JsonPropertyName("terminalId")]
    public string? TerminalId { get; set; }

    [JsonPropertyName("rrn")]
    public string? Rrn { get; set; }

    [JsonPropertyName("maskedPan")]
    public string? MaskedPan { get; set; }

    [JsonPropertyName("pspCode")]
    public string? PspCode { get; set; }

    [JsonPropertyName("pspName")]
    public string? PspName { get; set; }

    [JsonPropertyName("fpCode")]
    public string? FpCode { get; set; }

    [JsonPropertyName("fpName")]
    public string? FpName { get; set; }

    [JsonPropertyName("additionalInfo")]
    public DigipayVerifyAdditionalInfoModel? AdditionalInfo { get; set; }

    [JsonPropertyName("result")]
    public DigipayVerifyResultModel Result { get; set; }
}

public enum PaymentGatewayType
{
    IPG = 0,
    DPG = 1,
    WALLET = 3,
    CPG = 4
}

public class DigipayVerifyAdditionalInfoModel
{
    [JsonPropertyName("prepaymentAmount")]
    public long PrepaymentAmount { get; set; }

    [JsonPropertyName("cashAmount")]
    public long CashAmount { get; set; }

    [JsonPropertyName("creditAmount")]
    public long CreditAmount { get; set; }

    [JsonPropertyName("instantFinalization")]
    public bool InstantFinalization { get; set; }

    [JsonPropertyName("generateInvoice")]
    public bool GenerateInvoice { get; set; }
}

public class DigipayVerifyResultModel
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("level")]
    public string? Level { get; set; }
}