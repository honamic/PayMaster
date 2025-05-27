using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when payment amount is less than minimum limit
/// </summary>
public class GatewayMinAmountLimitException : BusinessException
{
    public GatewayMinAmountLimitException(decimal minAmount)
        : base($"حداقل مبلغ برای پرداخت در این درگاه {minAmount} است", PaymentErrorCodes.GatewayMinAmountLimit) { }
}
