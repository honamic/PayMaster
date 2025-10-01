using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.QueryModels.EntityFramework.PaymentGatewayProfiles;

internal class PaymentGatewayProfileQueryModelRepository : IPaymentGatewayProfileQueryModelRepository
{
    private readonly DbContext _context;

    public PaymentGatewayProfileQueryModelRepository([FromKeyedServices(QueryConstants.QueryDbContextKey)] DbContext context)
    {
        _context = context;
    }

    public Task<List<GetActivePaymentGatewaysQueryResult>> GetActivePaymentGatewayProfilesAsync(GetActivePaymentGatewaysQuery query, CancellationToken cancellationToken)
    {
        return _context.Set<PaymentGatewayProfileQueryModel>()
             .Where(c => c.Enabled)
             .Select(c => new GetActivePaymentGatewaysQueryResult
             {
                 Id = c.Id,
                 Code = c.Code,
                 Title = c.Title,
                 LogoPath = c.LogoPath,
                 MinimumAmount = c.MinimumAmount,
                 MaximumAmount = c.MaximumAmount
             }).ToListAsync(cancellationToken);
    }
}