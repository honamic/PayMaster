using Honamic.PayMaster.PaymentProvider.PayPal.Helper;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;

[JsonConverter(typeof(JsonEnumMemberStringEnumConverter))]
public enum PaypalExperienceUserAction
{
    /// <summary>
    ///After you redirect the customer to the PayPal payment page, a Continue button appears. Use this option when the final amount is not known when the checkout flow is initiated and you want to redirect the customer to the merchant page without processing the payment.
    /// Continue.
    /// </summary>
    [EnumMember(Value = "CONTINUE")]
    Continue,

    /// <summary>
    ///After you redirect the customer to the PayPal payment page, a Pay Now button appears. Use this option when the final amount is known when the checkout is initiated and you want to process the payment immediately when the customer clicks Pay Now.
    /// PayNow.
    /// </summary>
    [EnumMember(Value = "PAY_NOW")]
    PayNow,

    /// <summary>
    /// Unknown values will be mapped by this enum member.
    /// </summary>
    _Unknown
}