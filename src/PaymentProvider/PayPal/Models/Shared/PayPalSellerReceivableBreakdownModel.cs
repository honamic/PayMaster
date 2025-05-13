namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalSellerReceivableBreakdownModel
{
    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("gross_amount")]
    public PayPalMoney? GrossAmount { get; set; }

    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("paypal_fee")]
    public PayPalMoney? PaypalFee { get; set; }

    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("paypal_fee_in_receivable_currency")]
    public PayPalMoney? PaypalFeeInReceivableCurrency { get; set; }

    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("net_amount")]
    public PayPalMoney? NetAmount { get; set; }

    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("receivable_amount")]
    public PayPalMoney? ReceivableAmount { get; set; }

    /// <summary>
    /// The exchange rate that determines the amount to convert from one currency to another currency.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public PayPalExchangeRateModel? ExchangeRate { get; set; }

    /// <summary>
    /// An array of platform or partner fees, commissions, or brokerage fees that associated with the captured payment.
    /// </summary>
    [JsonPropertyName("platform_fees")]
    public List<PayPalPlatformFeeModel>? PlatformFees { get; set; }
}
