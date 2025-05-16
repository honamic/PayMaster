namespace Honamic.PayMaster.PaymentProvider.PayPal.Models;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using System.Text.Json.Serialization;

public class PaypalCreateOrder
{
    [JsonPropertyName("intent")]
    public required PayPalCheckoutPaymentIntent Intent { get; set; }

    [JsonPropertyName("purchase_units")]
    public required PayPalPurchaseUnit[] PurchaseUnits { get; set; }

    [JsonPropertyName("payment_source")]
    public required PaypalCreateOrderRequestPaymentSource PaymentSource { get; set; }
}

public class PaypalCreateOrderRequestPaymentSource
{
    [JsonPropertyName("paypal")]
    public required PaypalWalletModel Paypal { get; set; }
}
