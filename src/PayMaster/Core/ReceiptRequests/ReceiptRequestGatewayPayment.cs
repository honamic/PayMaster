using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.ReceiptRequests;

public class ReceiptRequestGatewayPayment : Entity<long>
{
    public decimal Amount { get; set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; set; } = default!;

    public PaymentGatewayStatus Status { get; set; }

    public PaymentGatewayFailedReason FailedReason { get; set; }

    public long GatewayProviderId { get; set; }
    public PaymentGatewayProvider GatewayProvider { get; set; } = default!;

    /// <summary>
    /// این مقدار هنگام ایجاد پرداخت به ما برای فرستادن کاربر به درگاه داده می شود.    
    /// </summary>
    public string? CreateReference { get; set; }

    /// <summary>
    /// زمان ارسال به درگاه
    /// </summary>
    public DateTimeOffset? RedirectAt { get; set; }

    /// <summary>
    /// زمان برگشت از درگاه
    /// </summary>
    public DateTimeOffset? CallBackAt { get; set; }

    /// <summary>
    /// بعضی از درگاه ها هنگام پرداخت موفق یه شناسه پرداخت موفق جدا می دهند
    /// مثلا در بانک سامان نام این فید رسید دیجیتال است
    /// </summary>
    public string? SuccessReference { get; set; }

    /// <summary>
    /// شماره مرجع 12 رقمی یا RRN
    /// </summary>
    public string? ReferenceRetrievalNumber { get; set; }

    /// <summary>
    /// شماره پیگیری 6 رقمی
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// شماره کارت
    /// </summary>
    public string? Pan { get; set; }

    public string? TerminalId { get; set; }

    public string? MerchantId { get; set; }
}