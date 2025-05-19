using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Core.ReceiptRequests.Parameters;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Core.ReceiptRequests;

public class ReceiptRequestGatewayPayment : Entity<long>
{
    public decimal Amount { get; private set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; private set; } = default!;

    public PaymentGatewayStatus Status { get; private set; }
    
    public string? StatusDescription { get; private set; }

    public PaymentGatewayFailedReason FailedReason { get; private set; }

    public long GatewayProviderId { get; set; }
    public PaymentGatewayProvider GatewayProvider { get; private set; } = default!;

    /// <summary>
    /// این مقدار هنگام ایجاد پرداخت به ما برای فرستادن کاربر به درگاه داده می شود.    
    /// </summary>
    public string? CreateReference { get; private set; }

    /// <summary>
    /// زمان ارسال به درگاه
    /// </summary>
    public DateTimeOffset? RedirectAt { get; private set; }

    /// <summary>
    /// زمان برگشت از درگاه
    /// </summary>
    public DateTimeOffset? CallBackAt { get; private set; }

    /// <summary>
    /// بعضی از درگاه ها هنگام پرداخت موفق یه شناسه پرداخت موفق جدا می دهند
    /// مثلا در بانک سامان نام این فید رسید دیجیتال است
    /// </summary>
    public string? SuccessReference { get; private set; }

    /// <summary>
    /// شماره مرجع 12 رقمی یا RRN
    /// </summary>
    public string? ReferenceRetrievalNumber { get; private set; }

    /// <summary>
    /// شماره پیگیری 6 رقمی
    /// </summary>
    public string? TrackingNumber { get; private set; }

    /// <summary>
    /// شماره کارت
    /// </summary>
    public string? Pan { get; private set; }

    public string? TerminalId { get; private set; }

    public string? MerchantId { get; private set; }

    internal static ReceiptRequestGatewayPayment Create(CreateGatewayPaymentParameters parameters)
    {
        if (!parameters.GatewayProvider.Enabled)
        {
            throw new ArgumentException($"درگاه انتخاب شده غیرفعال است.");
        }

        if (parameters.GatewayProvider.MinimumAmount.HasValue
           && parameters.Amount < parameters.GatewayProvider.MinimumAmount)
        {
            throw new ArgumentException($"حداقل مبلغ برای پرداخت در این درگاه {parameters.GatewayProvider.MinimumAmount} است");
        }

        if (parameters.GatewayProvider.MaximumAmount.HasValue
           && parameters.Amount > parameters.GatewayProvider.MaximumAmount)
        {
            throw new ArgumentException($"حداکثر مبلغ برای پرداخت در این درگاه {parameters.GatewayProvider.MaximumAmount} است");
        }

        var newReceiptRequestGatewayPayment = new ReceiptRequestGatewayPayment()
        {
            Id = parameters.Id,
            Amount = parameters.Amount,
            Currency = parameters.Currency,
            GatewayProviderId = parameters.GatewayProvider.Id,
            Status = PaymentGatewayStatus.New,
        };

        return newReceiptRequestGatewayPayment;
    }
}