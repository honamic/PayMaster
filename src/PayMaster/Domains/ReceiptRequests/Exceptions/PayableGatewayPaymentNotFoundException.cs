using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions; 

/// <summary>
/// Exception thrown when a payable gateway payment is not found
/// </summary>
public class PayableGatewayPaymentNotFoundException : BusinessException
{
    public PayableGatewayPaymentNotFoundException()
        : base("پرداخت درگاهی آماده پرداخت وجود ندارد.", PaymentErrorCodes.GatewayPaymentNotFound) { }
}
