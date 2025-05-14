using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Core.PaymentRequesters;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence;

internal class PaymentRequesterRepository
    : RepositoryBase<PaymentRequester, long>
    , IPaymentRequesterRepository
{
    public PaymentRequesterRepository(PaymasterDbContext context) : base(context)
    {

    }


    protected override IList<Expression<Func<PaymentRequester, object?>>> GetIncludes()
    {
        return new List<Expression<Func<PaymentRequester, object?>>>
        {
            // c=>c.Logs
        };
    }
}