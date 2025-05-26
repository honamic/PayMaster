using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProviders;

internal class PaymentGatewayProviderRepository 
    : RepositoryBase<PaymentGatewayProvider, long>
    , IPaymentGatewayProviderRepository
{
    public PaymentGatewayProviderRepository(PaymasterDbContext context) : base(context)
    {

    }


    protected override IList<Expression<Func<PaymentGatewayProvider, object?>>> GetIncludes()
    {
        return new List<Expression<Func<PaymentGatewayProvider, object?>>>
        {
            // c=>c.Logs
        };
    }
}
