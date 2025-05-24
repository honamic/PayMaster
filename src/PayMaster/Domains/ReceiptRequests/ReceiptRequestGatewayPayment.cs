using Honamic.Framework.Domain;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domains.ReceiptRequests;

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
    public DateTimeOffset? CallbackAt { get; private set; }

    public string? CallbackData { get; private set; }

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
    
    public long ReceiptRequestId { get;  set; }

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

    public void SetWaitingStatus(string? createReference, string? statusDescription, DateTimeOffset redirectAt)
    {
        Status = PaymentGatewayStatus.Waiting;
        RedirectAt = redirectAt;

        if (createReference?.Length > 128)
        {
            throw new ArgumentException("The number of CreateReference cannot be more than 128 characters.");
        }

        CreateReference = createReference;
        StatusDescription = ApplyStatusDescriptionMaxLength(statusDescription);
    }

    public void SetFailedStatus(PaymentGatewayFailedReason reason, string? statusDescription)
    {
        Status = PaymentGatewayStatus.Failed;
        FailedReason = reason;
        StatusDescription = ApplyStatusDescriptionMaxLength(statusDescription);
    }

    private static string? ApplyStatusDescriptionMaxLength(string? statusDescription)
    {
        if (statusDescription?.Length > 256)
            statusDescription = statusDescription.Substring(0, 256);

        return statusDescription;
    }

    internal void StartCallBack(DateTimeOffset nowWithOffset)
    {
        if (Status != PaymentGatewayStatus.Waiting)
        {
            throw new InvalidOperationException("Status Invalid");
        }

        CallbackAt = nowWithOffset;

        Status = PaymentGatewayStatus.Settlement;
    }

    internal void SuccessCallBack(SupplementaryPaymentInformation? supplementaryPaymentInformation)
    {
        Status = PaymentGatewayStatus.Success;
        FailedReason = PaymentGatewayFailedReason.None;

        if (supplementaryPaymentInformation is not null)
        {
            TrackingNumber = supplementaryPaymentInformation?.TrackingNumber;
            ReferenceRetrievalNumber = supplementaryPaymentInformation?.ReferenceRetrievalNumber;
            Pan = supplementaryPaymentInformation?.Pan;
            SuccessReference =supplementaryPaymentInformation?.SuccessReference;
            MerchantId = supplementaryPaymentInformation?.MerchantId ?? MerchantId;
            TerminalId = supplementaryPaymentInformation?.TerminalId ?? TerminalId;
        }
    }
   
    internal void FailedCallBack(PaymentGatewayFailedReason paymentFailedReason, string? statusDescription)
    {
        Status = PaymentGatewayStatus.Failed;
        StatusDescription = statusDescription;
        FailedReason = paymentFailedReason;
    }
}