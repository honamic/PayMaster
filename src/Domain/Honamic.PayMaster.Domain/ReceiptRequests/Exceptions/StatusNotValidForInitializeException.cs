using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

public class StatusNotValidForInitializeException : BusinessException
{
    public StatusNotValidForInitializeException(string message) : base(message)
    {

    }

    public StatusNotValidForInitializeException()
     : base("وضعیت قبض برای پرداخت معتبر نیست.", PaymentErrorCodes.PaymentStatusNotValidForInitialize) { }
}