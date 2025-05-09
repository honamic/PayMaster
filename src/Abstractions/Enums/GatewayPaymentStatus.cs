using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.Enums;

public enum GatewayPaymentStatus
{
    [Display(Name = "نامشخص")]
    None = 0,

    [Display(Name = "جدید")]
    New = 1,

    [Display(Name = "ناموفق")]
    Failed = 2,

    [Display(Name = "موفق")]
    Success = 3,

    [Display(Name = "انتظار پرداخت")]
    Waiting = 4,

    [Display(Name = "درحال تایید")]
    Settlement = 5,
}
