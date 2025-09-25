using Honamic.Framework.Domain;
using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProviders;

internal class PaymentGatewayProviderRepository 
    : RepositoryBase<PaymentGatewayProvider, long>
    , IPaymentGatewayProviderRepository
{
    public PaymentGatewayProviderRepository([FromKeyedServices(DomainConstants.PersistenceDbContextKey)] DbContext context) : base(context)
    {

    }

    public Task<PaymentGatewayProvider?> GetByCodeAsync(string code)
    {
        return GetAsync(c => c.Code == code);
    }

    public Task<PaymentGatewayProvider?> GetByIdAsync(long id)
    {
        return GetAsync(c => c.Id == id);
    }

    protected override IList<Expression<Func<PaymentGatewayProvider, object?>>> GetIncludes()
    {
        return new List<Expression<Func<PaymentGatewayProvider, object?>>>
        {
            // c=>c.Logs
        };
    }
}
