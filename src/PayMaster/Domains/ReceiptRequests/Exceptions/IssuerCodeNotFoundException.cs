using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception when the specified issuer code doesn't exist
/// </summary>
public class IssuerCodeNotFoundException : BusinessException
{
    public IssuerCodeNotFoundException(string? issuerCode)
        : base($"کد صادرکننده فیش وجود ندارد [{issuerCode}]", PaymentErrorCodes.IssuerCodeNotFound) { }
}
