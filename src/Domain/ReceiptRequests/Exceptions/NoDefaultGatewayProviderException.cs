using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when no default gateway provider is specified
/// </summary>
public class NoDefaultGatewayProviderException : BusinessException
{
    public NoDefaultGatewayProviderException()
        : base("درگاه پرداخت مشخص نشده است و پیش فرض هم مشخص نشده است.", PaymentErrorCodes.NoDefaultGatewayProvider) { }
}
