using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Core.ReceiptRequests;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence;

internal class ReceiptRequestRepository
    : RepositoryBase<ReceiptRequest, long>
    , IReceiptRequestRepository
{
    public ReceiptRequestRepository(PaymasterDbContext context) : base(context)
    {

    }


    protected override IList<Expression<Func<ReceiptRequest, object?>>> GetIncludes()
    {
        return new List<Expression<Func<ReceiptRequest, object?>>>
        {
            c=>c.GatewayPayments
        };
    }
}
