using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

public class PaymentStatusNotValidForProcessingException : BusinessException
{
    public PaymentStatusNotValidForProcessingException()
        : base("وضعیت برای پردازش این عملیات معتبر نیست.", PaymentErrorCodes.PaymentStatusNotValidForProcessing) { }
}
