using Honamic.PayMaster.PaymentProvider.PayPal.Helper;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;

[JsonConverter(typeof(JsonEnumMemberStringEnumConverter))]
public enum PayPalPhoneType
{
    /// <summary>
    /// Fax.
    /// </summary>
    [EnumMember(Value = "FAX")]
    Fax,

    /// <summary>
    /// Home.
    /// </summary>
    [EnumMember(Value = "HOME")]
    Home,

    /// <summary>
    /// Mobile.
    /// </summary>
    [EnumMember(Value = "MOBILE")]
    Mobile,

    /// <summary>
    /// Other.
    /// </summary>
    [EnumMember(Value = "OTHER")]
    Other,

    /// <summary>
    /// Pager.
    /// </summary>
    [EnumMember(Value = "PAGER")]
    Pager,

    /// <summary>
    /// Unknown values will be mapped by this enum member.
    /// </summary>
    _Unknown
}
