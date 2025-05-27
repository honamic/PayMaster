using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when payment status is not valid for processing
/// </summary>
public class PaymentStatusNotValidForProcessingException : BusinessException
{
    public PaymentStatusNotValidForProcessingException()
        : base("وضعیت برای پردازش معتبر نیست.", PaymentErrorCodes.PaymentStatusNotValidForProcessing) { }
}
