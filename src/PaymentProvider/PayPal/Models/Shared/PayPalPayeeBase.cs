namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalPayeeBase
{
    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }
}
