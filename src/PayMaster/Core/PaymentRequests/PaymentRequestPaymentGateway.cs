using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.PaymentRequests;

public class PaymentRequestPaymentGateway
{
    public long Id { get;  set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; set; } = default!;

    public GatewayPaymentStatus Status { get; set; }

    public PaymentFailedReason FailedReason { get; set; }

    public long GatewayProviderRef { get; set; }
    public PaymentGatewayProvider GatewayProvider { get; set; }

    /// <summary>
    /// این مقدار هنگام ایجاد پرداخت به ما برای فرستادن کاربر به درگاه داده می شود.    
    /// </summary>
    public string? GatewayCreateReference { get; set; }

    /// <summary>
    /// زمان ارسال به درگاه
    /// </summary>
    public DateTimeOffset? GatewayRedirectAt { get; set; }

    /// <summary>
    /// زمان برگشت از درگاه
    /// </summary>
    public DateTimeOffset? GatewayCallBackAt { get; set; }

    /// <summary>
    /// بعضی از درگاه ها هنگام پرداخت موفق یه شناسه پرداخت موفق جدا می دهند
    /// مثلا در بانک سامان نام این فید رسید دیجیتال است
    /// </summary>
    public string? GatewaySuccessReference { get; set; }

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