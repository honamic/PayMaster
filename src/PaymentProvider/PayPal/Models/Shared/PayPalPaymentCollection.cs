namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using System.Text.Json.Serialization;

public class PayPalPaymentCollection
{
    [JsonPropertyName("captures")]
    public PayPalOrdersCaptureModel[]? Captures { get; set; }


    //[JsonPropertyName("refunds")]
    //public List<object>? Refunds { get; set; }
}
