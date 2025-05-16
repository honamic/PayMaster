using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Core.ReceiptIssuers;

public interface IReceiptIssuerRepository
    : IRepositoryBase<ReceiptIssuer, long>
{

}
