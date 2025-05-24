using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Domains.ReceiptIssuers;

public interface IReceiptIssuerRepository
    : IRepositoryBase<ReceiptIssuer, long>
{

}
