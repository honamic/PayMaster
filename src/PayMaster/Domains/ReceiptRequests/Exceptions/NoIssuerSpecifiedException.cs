using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception when no issuer is specified or configured
/// </summary>
public class NoIssuerSpecifiedException : BusinessException
{
    public NoIssuerSpecifiedException()
        : base("صادر کننده فیش مشخص نشده است و پیش فرض هم مشخص نشده است.", PaymentErrorCodes.NoIssuerSpecified) { }
}
