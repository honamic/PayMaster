namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using System.Text.Json.Serialization;

public class PayPalTaxInfo
{
    /// <summary>
    /// The customer's tax ID value.
    /// </summary>
    [JsonPropertyName("tax_id")]
    public string? TaxId { get; set; }

    /// <summary>
    /// The customer's tax ID type.
    /// </summary>
    [JsonPropertyName("tax_id_type")]
    public PayPalTaxIdType TaxIdType { get; set; }
}