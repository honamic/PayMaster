using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when a gateway provider is not found
/// </summary>
public class GatewayProviderNotFoundException : BusinessException
{
    public GatewayProviderNotFoundException()
        : base("درگاه پرداخت شناسایی نشد.", PaymentErrorCodes.GatewayProviderNotFound) { }
}
