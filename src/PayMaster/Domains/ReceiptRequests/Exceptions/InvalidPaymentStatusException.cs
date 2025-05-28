using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when payment status is invalid for an operation
/// </summary>
public class InvalidPaymentStatusException : BusinessException
{
    public InvalidPaymentStatusException()
        : base("Status Invalid", PaymentErrorCodes.InvalidPaymentStatus) { }
}
