using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentRequestResponse
{
    [JsonPropertyName("authority")]
    public string? Authority { get; set; }

    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }

    [JsonPropertyName("fee_type")]
    public string? FeeType { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }
}