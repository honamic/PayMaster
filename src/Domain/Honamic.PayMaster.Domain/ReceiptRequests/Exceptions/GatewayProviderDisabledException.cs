using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when gateway provider is disabled
/// </summary>
public class GatewayProviderDisabledException : BusinessException
{
    public GatewayProviderDisabledException()
        : base("درگاه انتخاب شده غیرفعال است.", PaymentErrorCodes.GatewayProviderDisabled) { }
}
