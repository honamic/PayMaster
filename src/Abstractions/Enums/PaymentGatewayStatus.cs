using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.Enums;

public enum PaymentGatewayStatus
{
    [Display(Name = "نامشخص")]
    None = 0,

    [Display(Name = "جدید")]
    New = 1,

    [Display(Name = "ناموفق")]
    Failed = 2,

    [Display(Name = "انتظار پرداخت")]
    Waiting = 3,

    [Display(Name = "درحال تایید")]
    Settlement = 4,

    [Display(Name = "موفق")]
    Success = 5,

    [Display(Name = "درحال برگشت")]
    Reversing = 6,

    [Display(Name = "برگشت خورده")]
    Reversed = 7,
}