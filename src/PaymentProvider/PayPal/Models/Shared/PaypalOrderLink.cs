using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

public class PaypalOrderLink
{
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("rel")]
    public string Rel { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; }
}
