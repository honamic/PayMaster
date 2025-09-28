using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.ReceiptRequests;

public enum ReceiptRequestStatus
{
    [Display(Name = "نامشخص")]
    None = 0,

    [Display(Name = "جدید")]
    New = 1,

    [Display(Name = "در حال انجام")]
    Doing = 2,

    [Display(Name = "انجام شد")]
    Done = 3,

    [Display(Name = "لغو شد")]
    Canceled = 4,

    [Display(Name = "برگشت خورد")]
    Reversed = 5,
}