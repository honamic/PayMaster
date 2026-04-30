using Honamic.Framework.Domain;
using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers;

internal class ReceiptIssuerRepository
    : RepositoryBase<ReceiptIssuer, long>
    , IReceiptIssuerRepository
{
    public ReceiptIssuerRepository([FromKeyedServices(DomainConstants.PersistenceDbContextKey)] DbContext context) : base(context)
    {

    }

    public Task<ReceiptIssuer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return GetAsync(x => x.Code == code, cancellationToken);
    }

    protected override IList<Func<IQueryable<ReceiptIssuer>, IQueryable<ReceiptIssuer>>> GetIncludes()
    {
        return new List<Func<IQueryable<ReceiptIssuer>, IQueryable<ReceiptIssuer>>>
        {
            // c=>c.Logs
        };
    }
}