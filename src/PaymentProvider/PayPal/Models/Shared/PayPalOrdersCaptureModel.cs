namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using System.Text.Json.Serialization;

public class PayPalOrdersCaptureModel
{
    /// <summary>
    /// The PayPal-generated ID for the captured payment.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The status of the captured payment.
    /// </summary>
    [JsonPropertyName("status")]
    public PayPalCaptureStatus? Status { get; set; }

    /// <summary>
    /// The details of the captured payment status.
    /// </summary>
    [JsonPropertyName("status_details")]
    public PayPalCaptureStatusDetails? StatusDetails { get; set; }

    /// <summary>
    /// The currency and amount for a financial transaction, such as a balance or payment due.
    /// </summary>
    [JsonPropertyName("amount")]
    public PayPalMoney? Amount { get; set; }

    /// <summary>
    /// The API caller-provided external invoice number for this order. Appears in both the payer's transaction history and the emails that the payer receives.
    /// </summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>
    /// The API caller-provided external ID. Used to reconcile API caller-initiated transactions with PayPal transactions. Appears in transaction and settlement reports.
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }


    /// <summary>
    /// Indicates whether you can make additional captures against the authorized payment. Set to `true` if you do not intend to capture additional payments against the authorization. Set to `false` if you intend to capture additional payments against the authorization.
    /// </summary>
    [JsonPropertyName("final_capture")]
    public bool? FinalCapture { get; set; }

    /// <summary>
    /// The detailed breakdown of the capture activity. This is not available for transactions that are in pending state.
    /// </summary>
    [JsonPropertyName("seller_receivable_breakdown")]
    public PayPalSellerReceivableBreakdownModel? SellerReceivableBreakdown { get; set; }


    /// <summary>
    /// An array of related [HATEOAS links](/docs/api/reference/api-responses/#hateoas-links).
    /// </summary>
    [JsonPropertyName("links")]
    public List<PaypalOrderLink>? Links { get; set; }

    /// <summary>
    /// The date and time, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Seconds are required while fractional seconds are optional. Note: The regular expression provides guidance but does not reject all invalid dates.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTimeOffset? CreateTime { get; set; }

    /// <summary>
    /// The date and time, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Seconds are required while fractional seconds are optional. Note: The regular expression provides guidance but does not reject all invalid dates.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTimeOffset? UpdateTime { get; set; }
}