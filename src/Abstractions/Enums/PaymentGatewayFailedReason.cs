using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.Enums;

public enum PaymentGatewayFailedReason
{
    [Display(Name = "ندارد")]
    None = 0,

    [Display(Name = "خطا در ایجاد")]
    CreateFailed = 1,

    [Display(Name = "خطا در برگشت از بانک")]
    CallbackFailed = 2,

    [Display(Name = "انصراف کاربر")]
    Canceled = 3,

    [Display(Name = "تایید داخلی ناموفق")]
    InternalVerfiy = 4,

    [Display(Name = "استعلام ناموفق")]
    Verfiy = 5,

    [Display(Name = "تسویه ناموفق")]
    Settlement = 6,


    [Display(Name = "سایر")]
    Other = 9,
}