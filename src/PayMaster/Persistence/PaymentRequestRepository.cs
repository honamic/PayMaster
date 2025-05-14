using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Core.PaymentRequests;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence;

internal class PaymentRequestRepository
    : RepositoryBase<PaymentRequest, long>
    , IPaymentRequestRepository
{
    public PaymentRequestRepository(PaymasterDbContext context) : base(context)
    {

    }


    protected override IList<Expression<Func<PaymentRequest, object?>>> GetIncludes()
    {
        return new List<Expression<Func<PaymentRequest, object?>>>
        {
            // c=>c.Logs
        };
    }
}
