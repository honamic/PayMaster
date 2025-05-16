namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalPlatformFeeModel
{
    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("amount")]
    public PayPalMoney? Amount { get; set; }

    /// <summary>
    /// The details for the merchant who receives the funds and fulfills the order. The merchant is also known as the payee.
    /// </summary>
    [JsonPropertyName("payee")]
    public PayPalPayeeBase? Payee { get; set; }
}