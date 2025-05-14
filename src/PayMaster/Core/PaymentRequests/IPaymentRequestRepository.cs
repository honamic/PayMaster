using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Core.PaymentRequests;

public interface IPaymentRequestRepository
    : IRepositoryBase<PaymentRequest, long>
{

}
