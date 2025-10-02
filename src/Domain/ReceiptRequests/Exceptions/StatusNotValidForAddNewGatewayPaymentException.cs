using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

internal class StatusNotValidForAddNewGatewayPaymentException : BusinessException
{
    public StatusNotValidForAddNewGatewayPaymentException(string message) : base(message)
    {

    }

    public StatusNotValidForAddNewGatewayPaymentException()
     : base("وضعیت قبض برای پرداخت مجدد معتبر نیست.", PaymentErrorCodes.PaymentStatusNotValidForInitialize) { }
}