using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when specified gateway provider does not exist
/// </summary>
public class SpecifiedGatewayProviderNotFoundException : BusinessException
{
    public SpecifiedGatewayProviderNotFoundException()
        : base("درگاه پرداخت مشخص شده وجود ندارد.", PaymentErrorCodes.SpecifiedGatewayProviderNotFound) { }
}
