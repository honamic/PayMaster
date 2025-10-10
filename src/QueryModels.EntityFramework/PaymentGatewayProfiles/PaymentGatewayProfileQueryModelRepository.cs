using Honamic.Framework.EntityFramework.QueryModels;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Gridify;

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

    public Task<PagedQueryResult<GetAllPaymentGatewaysQueryResult>> GetAll(GetAllPaymentGatewaysQuery query, CancellationToken cancellationToken)
    {
        return _context.Set<PaymentGatewayProfileQueryModel>()
             .Select(c => new GetAllPaymentGatewaysQueryResult
             {
                 Id = c.Id,
                 Code = c.Code,
                 Title = c.Title,
                 LogoPath = c.LogoPath,
                 MinimumAmount = c.MinimumAmount,
                 MaximumAmount = c.MaximumAmount,
                 Enabled = c.Enabled, 
                 ProviderType = c.ProviderType,
             })
             .ApplyFiltering(query.Filter) 
             .ToPagedListAsync(query, cancellationToken);
    }

    public Task<GetPaymentGatewayQueryResult?> Get(GetPaymentGatewayQuery query, CancellationToken cancellationToken)
    {
        return _context.Set<PaymentGatewayProfileQueryModel>()
             .Where(c => c.Id == query.Id)
             .Select(c => new GetPaymentGatewayQueryResult
             {
                 Id = c.Id,
                 Code = c.Code,
                 Title = c.Title,
                 LogoPath = c.LogoPath,
                 MinimumAmount = c.MinimumAmount,
                 MaximumAmount = c.MaximumAmount,
                 Enabled = c.Enabled,
                 JsonConfigurations = c.JsonConfigurations,
                 ProviderType = c.ProviderType,
             }).FirstOrDefaultAsync(cancellationToken);
    }

}