using Honamic.Framework.Domain;
using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers;

internal class ReceiptIssuerRepository
    : RepositoryBase<ReceiptIssuer, long>
    , IReceiptIssuerRepository
{
    public ReceiptIssuerRepository([FromKeyedServices(DomainConstants.PersistenceDbContextKey)] DbContext context) : base(context)
    {

    }

    public Task<ReceiptIssuer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return GetAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ReceiptIssuer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return GetAsync(x => x.Code == code, cancellationToken);
    }

    protected override IList<Expression<Func<ReceiptIssuer, object?>>> GetIncludes()
    {
        return new List<Expression<Func<ReceiptIssuer, object?>>>
        {
            // c=>c.Logs
        };
    }
}