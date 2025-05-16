using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Core.ReceiptRequests;

public interface IReceiptRequestRepository
    : IRepositoryBase<ReceiptRequest, long>
{

}
