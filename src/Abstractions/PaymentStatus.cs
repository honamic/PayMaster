using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster;

public enum PaymentStatus
{
    [Display(Name = "نامشخص")]
    None = 0,

    [Display(Name = "جدید")]
    New = 1,

    [Display(Name = "انتظار پرداخت")]
    Waiting = 2,
   
    [Display(Name = "انصراف")]
    Canceled = 3,

    [Display(Name = "درحال تایید")]
    Settlement = 4,

    [Display(Name = "ناموفق")]
    Failed = 5,

    [Display(Name = "موفق")]
    Success = 7,

}