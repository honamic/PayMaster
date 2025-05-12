using Honamic.PayMaster.PaymentProvider.PayPal.Helper;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;

[JsonConverter(typeof(JsonEnumMemberStringEnumConverter))]
public enum PayPalCheckoutPaymentIntent
{
    /// <summary>
    ///The merchant intends to capture payment immediately after the customer makes a payment.
    /// Capture.
    /// </summary>
    [EnumMember(Value = "CAPTURE")]
    Capture,

    /// <summary>
    ///The merchant intends to authorize a payment and place funds on hold after the customer makes a payment. Authorized payments are best captured within three days of authorization but are available to capture for up to 29 days. After the three-day honor period, the original authorized payment expires and you must re-authorize the payment. You must make a separate request to capture payments on demand. This intent is not supported when you have more than one `purchase_unit` within your order.
    /// Authorize.
    /// </summary>
    [EnumMember(Value = "AUTHORIZE")]
    Authorize,

    /// <summary>
    /// Unknown values will be mapped by this enum member.
    /// </summary>
    _Unknown
}
