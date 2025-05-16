using Honamic.PayMaster.PaymentProvider.PayPal.Helper;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;

[JsonConverter(typeof(JsonEnumMemberStringEnumConverter))]
public enum PayPalPayeePaymentMethodPreference
{
    /// <summary>
    ///Accepts any type of payment from the customer.
    /// Unrestricted.
    /// </summary>
    [EnumMember(Value = "UNRESTRICTED")]
    Unrestricted,

    /// <summary>
    ///Accepts only immediate payment from the customer. For example, credit card, PayPal balance, or instant ACH. Ensures that at the time of capture, the payment does not have the `pending` status.
    /// ImmediatePaymentRequired.
    /// </summary>
    [EnumMember(Value = "IMMEDIATE_PAYMENT_REQUIRED")]
    ImmediatePaymentRequired,

    /// <summary>
    /// Unknown values will be mapped by this enum member.
    /// </summary>
    _Unknown
}