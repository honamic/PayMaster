
namespace Honamic.PayMaster.Domains.ReceiptIssuers;

public interface IReceiptIssuerRepository
{
    Task<ReceiptIssuer?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ReceiptIssuer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task InsertAsync(ReceiptIssuer entity, CancellationToken cancellationToken = default);
}
