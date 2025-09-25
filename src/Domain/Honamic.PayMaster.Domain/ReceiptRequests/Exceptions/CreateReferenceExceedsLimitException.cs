using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when create reference exceeds length limit
/// </summary>
public class CreateReferenceExceedsLimitException : BusinessException
{
    public CreateReferenceExceedsLimitException()
        : base("The number of CreateReference cannot be more than 128 characters.", PaymentErrorCodes.CreateReferenceExceedsLimit) { }
}
