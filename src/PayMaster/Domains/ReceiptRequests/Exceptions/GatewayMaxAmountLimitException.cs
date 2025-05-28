using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when payment amount exceeds maximum limit
/// </summary>
public class GatewayMaxAmountLimitException : BusinessException
{
    public GatewayMaxAmountLimitException(decimal maxAmount)
        : base($"حداکثر مبلغ برای پرداخت در این درگاه {maxAmount} است", PaymentErrorCodes.GatewayMaxAmountLimit) { }
}
