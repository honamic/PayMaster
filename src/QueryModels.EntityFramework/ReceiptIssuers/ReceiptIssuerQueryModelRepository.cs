using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers;

internal class ReceiptIssuerQueryModelRepository : IReceiptIssuerQueryModelRepository
{ 
    private readonly DbContext _context;

    public ReceiptIssuerQueryModelRepository ([FromKeyedServices(QueryConstants.QueryDbContextKey)] DbContext context)
    {
        _context = context;
    }
}