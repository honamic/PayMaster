using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;

namespace Honamic.PayMaster.Application.ReceiptRequests.QueryHandlers;

public class GetPublicReceiptRequestQueryHandler : IQueryHandler<GetPublicReceiptRequestQuery, Result<GetPublicReceiptRequestQueryResult?>>
{
    private readonly IReceiptRequestQueryModelRepository _receiptRequestQueryModelRepository;

    public GetPublicReceiptRequestQueryHandler(IReceiptRequestQueryModelRepository receiptRequestQueryModelRepository)
    {
        _receiptRequestQueryModelRepository = receiptRequestQueryModelRepository;
    }

    public async Task<Result<GetPublicReceiptRequestQueryResult?>> HandleAsync(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken)
    {
       return await  _receiptRequestQueryModelRepository.GetPublicAsync(query, cancellationToken);
    }
}