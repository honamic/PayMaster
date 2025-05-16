using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
public class PayPalAddressModel
{
    /// <summary>
    /// The first line of the address, such as number and street, for example, `173 Drury Lane`. Needed for data entry, and Compliance and Risk checks. This field needs to pass the full address.
    /// </summary>
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// The second line of the address, for example, a suite or apartment number.
    /// </summary>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// A city, town, or village. Smaller than `admin_area_level_1`.
    /// </summary>
    [JsonPropertyName("admin_area_2")]
    public string? AdminArea2 { get; set; }

    /// <summary>
    /// The highest-level sub-division in a country, which is usually a province, state, or ISO-3166-2 subdivision. This data is formatted for postal delivery, for example, `CA` and not `California`. Value, by country, is: UK. A county. US. A state. Canada. A province. Japan. A prefecture. Switzerland. A *kanton*.
    /// </summary>
    [JsonPropertyName("admin_area_1")]
    public string? AdminArea1 { get; set; }

    /// <summary>
    /// The postal code, which is the ZIP code or equivalent. Typically required for countries with a postal code or an equivalent. See [postal code](https://en.wikipedia.org/wiki/Postal_code).
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// The [2-character ISO 3166-1 code](/api/rest/reference/country-codes/) that identifies the country or region. Note: The country code for Great Britain is GB and not UK as used in the top-level domain names for that country. Use the `C2` country code for China worldwide for comparable uncontrolled price (CUP) method, bank card, and cross-border transactions.
    /// </summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
}
