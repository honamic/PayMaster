namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalMoney
{
    [JsonPropertyName("currency_code")]
    public string? CurrencyCode { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
