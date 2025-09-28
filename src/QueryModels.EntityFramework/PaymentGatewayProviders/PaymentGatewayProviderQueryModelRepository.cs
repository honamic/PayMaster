using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProviders;

internal class PaymentGatewayProviderQueryModelRepository : IPaymentGatewayProviderQueryModelRepository
{
    private readonly DbContext _context;

    public PaymentGatewayProviderQueryModelRepository([FromKeyedServices(QueryConstants.QueryDbContextKey)] DbContext context)
    {
        _context = context;
    }

}
