namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;

/// <summary>
/// جدول ۵- Request Fields
/// https://www.mydigipay.com/developers/docs/upg
/// </summary>
public class TicketRequestDto
{
    /// <summary>
    /// مبلغ خرید
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    /// شماره همراه کاربر
    /// </summary>
    public required string CellNumber { get; set; }

    /// <summary>
    /// ایدی یونیک که از سمت شما برای این خرید ثبت می‌شود
    /// </summary>
    public required string ProviderId { get; set; }

    /// <summary>
    ///  آدرس برگشت به سایت پذیرنده
    /// </summary>
    public required string CallbackUrl { get; set; }

    public TicketRequestBasketDetailsDto? BasketDetails { get; set; } // فقط برای خریدهای اعتباری و اقساطی اجباری است
    
    public List<TicketRequestSplitDetailsDto>? SplitDetailsList { get; set; } // برای خریدهای تسهیمی، حداکثر سایز لیست 2 می‌باشد

    /// <summary>
    /// preferredGateway = PreferredGateway
    /// </summary>
    public Dictionary<string, string>? AdditionalInfo { get; set; } // توضیحات اضافه، مقداردهی اختیاری
}