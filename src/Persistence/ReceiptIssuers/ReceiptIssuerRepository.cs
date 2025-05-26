using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domains.ReceiptIssuers;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers;

internal class ReceiptIssuerRepository
    : RepositoryBase<ReceiptIssuer, long>
    , IReceiptIssuerRepository
{
    public ReceiptIssuerRepository(PaymasterDbContext context) : base(context)
    {

    }


    protected override IList<Expression<Func<ReceiptIssuer, object?>>> GetIncludes()
    {
        return new List<Expression<Func<ReceiptIssuer, object?>>>
        {
            // c=>c.Logs
        };
    }
}