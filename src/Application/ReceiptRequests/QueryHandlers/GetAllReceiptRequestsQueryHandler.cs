using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;

namespace Honamic.PayMaster.Application.ReceiptRequests.QueryHandlers;

public class GetAllReceiptRequestsQueryHandler : IQueryHandler<GetAllReceiptRequestsQuery, Result<PagedQueryResult<GetAllReceiptRequestsQueryResult>>>
{
    private readonly IReceiptRequestQueryModelRepository _receiptRequestQueryModelRepository;

    public GetAllReceiptRequestsQueryHandler(IReceiptRequestQueryModelRepository receiptRequestQueryModelRepository)
    {
        _receiptRequestQueryModelRepository = receiptRequestQueryModelRepository;
    }

    public async Task<Result<PagedQueryResult<GetAllReceiptRequestsQueryResult>>> HandleAsync(GetAllReceiptRequestsQuery query, CancellationToken cancellationToken)
    {
        return await _receiptRequestQueryModelRepository.GetAll(query, cancellationToken);
    }
}