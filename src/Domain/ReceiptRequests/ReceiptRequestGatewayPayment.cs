using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.Domain.ReceiptRequests;

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

    public long PaymentGatewayProfileId { get; set; } 

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

    public long ReceiptRequestId { get; set; }

    internal static ReceiptRequestGatewayPayment Create(CreateGatewayPaymentParameters parameters)
    {
        if (string.IsNullOrEmpty(parameters.Currency))
        {
            throw new CurrencyRequiredException();
        }

        if (!parameters.GatewayProvider.Enabled)
        {
            throw new GatewayProviderDisabledException();
        }

        if (parameters.GatewayProvider.MinimumAmount.HasValue
           && parameters.Amount < parameters.GatewayProvider.MinimumAmount)
        {
            throw new GatewayMinAmountLimitException(parameters.GatewayProvider.MinimumAmount.Value);
        }

        if (parameters.GatewayProvider.MaximumAmount.HasValue
           && parameters.Amount > parameters.GatewayProvider.MaximumAmount)
        {
            throw new GatewayMaxAmountLimitException(parameters.GatewayProvider.MaximumAmount.Value);
        }

        var newReceiptRequestGatewayPayment = new ReceiptRequestGatewayPayment()
        {
            Id = parameters.Id,
            Amount = parameters.Amount,
            Currency = parameters.Currency,
            PaymentGatewayProfileId = parameters.GatewayProvider.Id,
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
            throw new CreateReferenceExceedsLimitException();
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

    internal bool CanProcessCallback()
    {
        return Status == PaymentGatewayStatus.Waiting;
    }

    internal void SetCallback(DateTimeOffset nowWithOffset, string callBackData)
    {
        if (!CanProcessCallback())
        {
            throw new InvalidPaymentStatusException();
        }

        CallbackAt = nowWithOffset;
        CallbackData = callBackData;
        Status = PaymentGatewayStatus.Settlement;
    }

    internal void FailedCallback(PaymentGatewayFailedReason paymentFailedReason, string? statusDescription)
    {
        Status = PaymentGatewayStatus.Failed;
        StatusDescription = statusDescription;
        FailedReason = paymentFailedReason;
    }

    internal void SuccessVerify(SupplementaryPaymentInformation? supplementaryPaymentInformation)
    {
        if (Status != PaymentGatewayStatus.Settlement &&
            Status != PaymentGatewayStatus.Waiting)
        {
            throw new InvalidPaymentStatusException();
        }

        Status = PaymentGatewayStatus.Success;
        FailedReason = PaymentGatewayFailedReason.None;

        if (supplementaryPaymentInformation is not null)
        {
            TrackingNumber = supplementaryPaymentInformation?.TrackingNumber;
            ReferenceRetrievalNumber = supplementaryPaymentInformation?.ReferenceRetrievalNumber;
            Pan = supplementaryPaymentInformation?.Pan;
            SuccessReference = supplementaryPaymentInformation?.SuccessReference;
            MerchantId = supplementaryPaymentInformation?.MerchantId ?? MerchantId;
            TerminalId = supplementaryPaymentInformation?.TerminalId ?? TerminalId;
        }
    }

    internal void FailedVerify(PaymentGatewayFailedReason paymentFailedReason, string? statusDescription)
    {
        FailedCallback(paymentFailedReason, statusDescription);
    }
}