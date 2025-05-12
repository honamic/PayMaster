using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models;

public class PayPalCallBackDataModel
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }


    [JsonPropertyName("PayerID")]
    public string? PayerId { get; set; } 
}
