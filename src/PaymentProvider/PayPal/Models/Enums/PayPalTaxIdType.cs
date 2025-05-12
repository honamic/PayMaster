using Honamic.PayMaster.PaymentProvider.PayPal.Helper;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;

[JsonConverter(typeof(JsonEnumMemberStringEnumConverter))]
public enum PayPalTaxIdType
{
    /// <summary>
    ///The individual tax ID type, typically is 11 characters long.
    /// BrCpf.
    /// </summary>
    [EnumMember(Value = "BR_CPF")]
    BrCpf,

    /// <summary>
    ///The business tax ID type, typically is 14 characters long.
    /// BrCnpj.
    /// </summary>
    [EnumMember(Value = "BR_CNPJ")]
    BrCnpj,

    /// <summary>
    /// Unknown values will be mapped by this enum member.
    /// </summary>
    _Unknown
}