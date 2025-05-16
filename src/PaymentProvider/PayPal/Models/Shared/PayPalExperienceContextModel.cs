namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using System.Text.Json.Serialization;

public class PayPalExperienceContextModel
{
    /// <summary>
    /// The label that overrides the business name in the PayPal account on the PayPal site. The pattern is defined by an external party and supports Unicode.
    /// </summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>
    /// The [language tag](https://tools.ietf.org/html/bcp47#section-2) for the language in which to localize the error-related strings, such as messages, issues, and suggested actions. The tag is made up of the [ISO 639-2 language code](https://www.loc.gov/standards/iso639-2/php/code_list.php), the optional [ISO-15924 script tag](https://www.unicode.org/iso15924/codelists.html), and the [ISO-3166 alpha-2 country code](/api/rest/reference/country-codes/) or [M49 region code](https://unstats.un.org/unsd/methodology/m49/).
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    /// <summary>
    /// Describes the URL.
    /// </summary>
    [JsonPropertyName("return_url")]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Describes the URL.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; set; }


    /// <summary>
    /// The merchant-preferred payment methods.
    /// </summary>
    [JsonPropertyName("payment_method_preference")]
    public PayPalPayeePaymentMethodPreference? PaymentMethodPreference { get; set; }
 
    /// <summary>
    /// Configures a Continue or Pay Now checkout flow.
    /// </summary>
    [JsonPropertyName("user_action")]
    public PaypalExperienceUserAction? UserAction { get; set; }
}
