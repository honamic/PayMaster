using Honamic.Framework.Domain;
using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProfiles;

internal class PaymentGatewayProfileRepository
    : RepositoryBase<PaymentGatewayProfile, long>
    , IPaymentGatewayProfileRepository
{
    public PaymentGatewayProfileRepository([FromKeyedServices(DomainConstants.PersistenceDbContextKey)] DbContext context) : base(context)
    {

    }

    public Task<PaymentGatewayProfile?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return GetAsync(c => c.Code == code, cancellationToken);
    }
 
    public Task<bool> ExistsByCodeAsync(string code, long? currentId, CancellationToken cancellationToken = default)
    {
        if (currentId.HasValue)
            return IsExistsAsync(c => c.Code == code && c.Id != currentId.Value, cancellationToken);
        else
            return IsExistsAsync(c => c.Code == code, cancellationToken);

    }

    protected override IList<Func<IQueryable<PaymentGatewayProfile>, IQueryable<PaymentGatewayProfile>>> GetIncludes()
    {
        return new List<Func<IQueryable<PaymentGatewayProfile>, IQueryable<PaymentGatewayProfile>>>
        {
            // c=>c.Logs
        };
    }
}
