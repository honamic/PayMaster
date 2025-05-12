namespace Honamic.PayMaster.PaymentProvider.PayPal.Dtos;

using System.Text.Json.Serialization;

public class PaypalCreateOrder
{
    [JsonPropertyName("intent")]
    public required string Intent { get; set; }

    [JsonPropertyName("purchase_units")]
    public required PurchaseUnit[] PurchaseUnits { get; set; }

    [JsonPropertyName("payment_source")]
    public required PaymentSourceList PaymentSource { get; set; }
}

public class PaymentSourceList
{
    [JsonPropertyName("paypal")]
    public required PaymentSource Paypal { get; set; }
}

public class PaymentSource
{
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("payment_method_preference")]
    public required string PaymentMethodPreference { get; set; }

    [JsonPropertyName("experience_context")]
    public required ExperienceContext ExperienceContext { get; set; }
}

public class Address
{
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("admin_area_1")]
    public string? AdminArea1 { get; set; }

    [JsonPropertyName("admin_area_2")]
    public string? AdminArea2 { get; set; }

    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
}

public class ExperienceContext
{
    [JsonPropertyName("return_url")]
    public required string ReturnUrl { get; set; }

    [JsonPropertyName("cancel_url")]
    public required string CancelUrl { get; set; }
}

public class PurchaseUnit
{
    [JsonPropertyName("reference_id")]
    public required string ReferenceId { get; set; }

    [JsonPropertyName("amount")]
    public required Amount Amount { get; set; }
}

public class Amount
{
    [JsonPropertyName("currency_code")]
    public required string CurrencyCode { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }
}