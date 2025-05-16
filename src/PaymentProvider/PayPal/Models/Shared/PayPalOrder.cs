using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using System.Text.Json.Serialization;

public class PayPalOrder
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("create_time")]
    public DateTimeOffset? CreateTime { get; set; }

    [JsonPropertyName("status")]
    public PayPalOrderStatus? Status { get; set; }

    [JsonPropertyName("intent")]
    public required PayPalCheckoutPaymentIntent Intent { get; set; }

    [JsonPropertyName("purchase_units")]
    public required PayPalPurchaseUnit[] PurchaseUnits { get; set; }

    [JsonPropertyName("payment_source")]
    public PayPalOrderPaymentSource? PaymentSource { get; set; }

    [JsonPropertyName("links")]
    public PaypalOrderLink[]? Links { get; set; }
}
 