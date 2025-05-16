using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models;

public class PaypalCreateOrderResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public PayPalOrderStatus Status { get; set; }

    [JsonPropertyName("payment_source")]
    public PayPalOrderPaymentSource? PaymentSource { get; set; }

    [JsonPropertyName("payer")]
    public PaypalCreateOrderResponsePayer? Payer { get; set; }

    [JsonPropertyName("links")]
    public PaypalOrderLink[]? Links { get; set; }
}

 
public class PaypalCreateOrderResponsePayer
{
    [JsonPropertyName("address")]
    public PayPalAddressModel? Address { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("payment_method_preference")]
    public string? PaymentMethodPreference { get; set; }

    [JsonPropertyName("experience_context")]
    public PayPalExperienceContextModel? ExperienceContext { get; set; }
}
