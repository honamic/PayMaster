using Honamic.PayMaster.Core.ReceiptRequests;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Core.Services;
public interface IPaymentProcessingService
{
    Task<CreateResult> PreparePaymentAsync(ReceiptRequest receiptRequest, string callbackUrl);
}