using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using System.Text.Json.Serialization;

public class PayPalOrderPaymentSource
{
    [JsonPropertyName("paypal")]
    public PaypalWalletModel? Paypal { get; set; }
}
 