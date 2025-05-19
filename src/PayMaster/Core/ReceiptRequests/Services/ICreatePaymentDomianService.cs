using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Core.ReceiptRequests.Services;
public interface ICreatePaymentDomianService
{
    Task<CreateResult> CreatePaymentAsync(ReceiptRequest receiptRequest, string callbackUrl);
}