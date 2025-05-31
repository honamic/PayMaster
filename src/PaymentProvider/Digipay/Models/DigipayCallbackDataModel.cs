using Honamic.PayMaster.PaymentProvider.Digipay.Models.Enums;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;
public class DigipayCallbackDataModel
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("type")]
    public TicketType Type { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("providerId")]
    public string? ProviderId { get; set; }

    [JsonPropertyName("trackingCode")]
    public string? TrackingCode { get; set; }

    [JsonPropertyName("rrn")]
    public string? RRN { get; set; }

    [JsonPropertyName("isCredit")]
    public bool IsCredit { get; set; }

    [JsonPropertyName("psp")]
    public string? Psp { get; set; }

    [JsonPropertyName("creditProvider")]
    public string? CreditProvider { get; set; }
}