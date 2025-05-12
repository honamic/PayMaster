namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using System.Text.Json.Serialization;

public class PaypalWalletModel
{
    [JsonPropertyName("address")]
    public PayPalAddressModel? Address { get; set; }

    /// <summary>
    /// The internationalized email address. Note: Up to 64 characters are allowed before and 255 characters are allowed after the @ sign. However, the generally accepted maximum length for an email address is 254 characters. The pattern verifies that an unquoted @ sign exists.
    /// </summary>
    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }


    /// <summary>
    /// Customizes the payer experience during the approval process for payment with PayPal. Note: Partners and Marketplaces might configure brand_name and shipping_preference during partner account setup, which overrides the request values.
    /// </summary>
    [JsonPropertyName("experience_context")]
    public PayPalExperienceContextModel? ExperienceContext { get; set; }


    /// <summary>
    /// The PayPal-generated ID for the vaulted payment source. This ID should be stored on the merchant's server so the saved payment source can be used for future transactions.
    /// </summary>
    [JsonPropertyName("vault_id")]
    public string? VaultId { get; set; }


    /// <summary>
    /// The name of the party.
    /// </summary>
    [JsonPropertyName("name")]
    public PayPalName? Name { get; set; }

    /// <summary>
    /// The phone information.
    /// </summary>
    [JsonPropertyName("phone")]
    public PayPalPhoneType? Phone { get; set; }

    /// <summary>
    /// The stand-alone date, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). To represent special legal values, such as a date of birth, you should use dates with no associated time or time-zone data. Whenever possible, use the standard `date_time` type. This regular expression does not validate all dates. For example, February 31 is valid and nothing is known about leap years.
    /// </summary>
    [JsonPropertyName("birth_date"  )]
    public string? BirthDate { get; set; }

    /// <summary>
    /// The tax ID of the customer. The customer is also known as the payer. Both `tax_id` and `tax_id_type` are required.
    /// </summary>
    [JsonPropertyName("tax_info")]
    public PayPalTaxInfo? TaxInfo { get; set; }

}
