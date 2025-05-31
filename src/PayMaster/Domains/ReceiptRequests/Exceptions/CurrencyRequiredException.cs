using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;

/// <summary>
/// Exception thrown when a required currency is not specified
/// </summary>
public class CurrencyRequiredException : BusinessException
{
    public CurrencyRequiredException() 
        : base("واحد مشخص نشده است.")
    {
    }
}