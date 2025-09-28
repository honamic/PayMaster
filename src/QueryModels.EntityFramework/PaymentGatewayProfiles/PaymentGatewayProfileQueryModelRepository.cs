using Honamic.Framework.Queries;
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

}
