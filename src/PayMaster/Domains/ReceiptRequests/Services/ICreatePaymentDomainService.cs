using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Services;
public interface ICreatePaymentDomainService
{
    Task<CreateResult> CreatePaymentAsync(ReceiptRequest receiptRequest);
}