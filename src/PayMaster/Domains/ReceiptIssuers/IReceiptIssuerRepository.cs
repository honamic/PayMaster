
namespace Honamic.PayMaster.Domains.ReceiptIssuers;

public interface IReceiptIssuerRepository
{
    Task<ReceiptIssuer?> GetByIdAsync(long id);
    Task<ReceiptIssuer?> GetByCodeAsync(string code);
    Task InsertAsync(ReceiptIssuer entity);
}
