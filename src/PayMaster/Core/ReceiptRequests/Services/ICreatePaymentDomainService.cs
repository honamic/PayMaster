using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Core.ReceiptRequests.Services;
public interface ICreatePaymentDomainService
{
    Task<CreateResult> CreatePaymentAsync(ReceiptRequest receiptRequest);
}