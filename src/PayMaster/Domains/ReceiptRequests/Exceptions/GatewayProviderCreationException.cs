using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when a gateway provider cannot be created
/// </summary>
public class GatewayProviderCreationException : BusinessException
{
    public GatewayProviderCreationException()
        : base("درگاه پرداخت ساخته نشد.", PaymentErrorCodes.GatewayProviderCreationFailed) { }
}
