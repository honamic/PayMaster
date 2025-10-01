using Honamic.PayMaster.Application.ReceiptRequests.Queries;

namespace Honamic.PayMaster.QueryModels.ReceiptRequests;

public interface IReceiptRequestQueryModelRepository
{ 
    public Task<GetPublicReceiptRequestQueryResult?> GetPublicAsync(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken);
}