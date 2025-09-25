using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when payment is not valid
/// </summary>
public class InvalidPaymentException : BusinessException
{
    public InvalidPaymentException()
        : base("پرداخت معتبر نیست.", PaymentErrorCodes.InvalidPayment) { }
}
