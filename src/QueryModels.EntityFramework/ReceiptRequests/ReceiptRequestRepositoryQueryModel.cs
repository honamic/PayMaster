using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.QueryModels.EntityFramework.ReceiptRequests;

internal class ReceiptRequestRepositoryQueryModel : IReceiptRequestQueryModelRepository
{
    private readonly DbContext _context;

    public ReceiptRequestRepositoryQueryModel([FromKeyedServices(QueryConstants.QueryDbContextKey)] DbContext context)
    {
        _context = context;
    }
}