using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster;

public enum PaymentFailedReason
{
    [Display(Name = "انصراف کاربر")]
    Canceled = 1,

    [Display(Name = "تایید داخلی ناموفق")]
    InternalVerfiy = 2,

    [Display(Name = "استعلام ناموفق")]
    Verfiy = 3,

    [Display(Name = "تسویه ناموفق")]
    Settlement = 4,

    [Display(Name = "سایر")]
    Other = 5,
}