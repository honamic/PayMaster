using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Core.PaymentRequesters;

public interface IPaymentRequesterRepository
    : IRepositoryBase<PaymentRequester, long>
{

}
