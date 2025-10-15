using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;

namespace Honamic.PayMaster.QueryModels.ReceiptRequests;

public interface IReceiptRequestQueryModelRepository
{
    Task<PagedQueryResult<GetAllReceiptRequestsQueryResult>> GetAll(GetAllReceiptRequestsQuery query, CancellationToken cancellationToken);
    Task<GetPublicReceiptRequestQueryResult?> GetPublicAsync(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken);
}