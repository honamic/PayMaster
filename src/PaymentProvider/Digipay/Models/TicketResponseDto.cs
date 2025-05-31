namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;

public class TicketResponseDto
{
    /// <summary>
    /// تیکت ساخته برای این خرید	
    /// </summary>
    public string? Ticket { get; set; }

    /// <summary>
    /// آدرسی که از طریق آن کاربر به درگاه پرداخت دیجی پی ریدایرکت میشود.	
    /// </summary>
    public string? RedirectUrl { get; set; }
    
    
    public TicketResponseResultDto Result { get; set; }

    /// <summary>
    /// اطلاعات بیمه
    /// </summary>
    public TicketResponseInsurancePoliciesDto? InsurancePolicies { get; set; }
}


public class TicketResponseResultDto
{
    /// <summary>
    /// کد عددی وضعیت پاسخ
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    ///  پیام توصیف وضعیت پاسخ
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// ---------	
    /// </summary>
    public string? Level { get; set; }
}

public class TicketResponseInsurancePoliciesDto
{
    /// <summary>
    /// کد بیمه
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// شناسه پیش‌نویس بیمه‌نامه
    /// </summary>
    public string? PolicyDraftNo { get; set; } 
}