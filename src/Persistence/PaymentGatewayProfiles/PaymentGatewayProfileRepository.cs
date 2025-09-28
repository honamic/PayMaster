using Honamic.Framework.Domain;
using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProfiles;

internal class PaymentGatewayProfileRepository 
    : RepositoryBase<PaymentGatewayProfile, long>
    , IPaymentGatewayProfileRepository
{
    public PaymentGatewayProfileRepository([FromKeyedServices(DomainConstants.PersistenceDbContextKey)] DbContext context) : base(context)
    {

    }

    public Task<PaymentGatewayProfile?> GetByCodeAsync(string code)
    {
        return GetAsync(c => c.Code == code);
    }

    public Task<PaymentGatewayProfile?> GetByIdAsync(long id)
    {
        return GetAsync(c => c.Id == id);
    }

    protected override IList<Expression<Func<PaymentGatewayProfile, object?>>> GetIncludes()
    {
        return new List<Expression<Func<PaymentGatewayProfile, object?>>>
        {
            // c=>c.Logs
        };
    }
}
