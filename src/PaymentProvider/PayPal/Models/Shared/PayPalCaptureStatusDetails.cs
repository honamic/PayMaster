namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;

using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using System.Text.Json.Serialization;

public class PayPalCaptureStatusDetails
{
    /// <summary>
    /// The reason why the captured payment status is `PENDING` or `DENIED`.
    /// </summary>
    [JsonPropertyName("reason")]
    public PayPalCaptureIncompleteReason? Reason { get; set; }
}
