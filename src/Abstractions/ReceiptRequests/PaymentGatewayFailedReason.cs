using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.ReceiptRequests;

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
    InternalVerify = 4,

    [Display(Name = "استعلام ناموفق")]
    Verify = 5,

    [Display(Name = "تسویه ناموفق")]
    Settlement = 6,


    [Display(Name = "سایر")]
    Other = 9,
}