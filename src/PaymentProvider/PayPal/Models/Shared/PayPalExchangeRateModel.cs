namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalExchangeRateModel
{

    /// <summary>
    /// The [three-character ISO-4217 currency code](/api/rest/reference/currency-codes/) that identifies the currency.
    /// </summary>
    [JsonPropertyName("source_currency")]
    public string? SourceCurrency { get; set; }

    /// <summary>
    /// The [three-character ISO-4217 currency code](/api/rest/reference/currency-codes/) that identifies the currency.
    /// </summary>
    [JsonPropertyName("target_currency")]
    public string? TargetCurrency { get; set; }

    /// <summary>
    /// The target currency amount. Equivalent to one unit of the source currency. Formatted as integer or decimal value with one to 15 digits to the right of the decimal point.
    /// </summary>
    [JsonPropertyName("value")]
    public string? MValue { get; set; }

}
