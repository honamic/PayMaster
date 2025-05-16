namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalPurchaseUnit
{
    [JsonPropertyName("reference_id")]
    public string? ReferenceId { get; set; }

    [JsonPropertyName("amount")]
    public PayPalMoney? Amount { get; set; }


    [JsonPropertyName("payee")]
    public PayPalPayeeBase? Payee { get; set; }


    [JsonPropertyName("payments")]
    public PayPalPaymentCollection? Payments { get; set; }
}
