using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;

public class GetAllPaymentGatewaysQueryHandler : IQueryHandler<GetAllPaymentGatewaysQuery, Result<PagedQueryResult<GetAllPaymentGatewaysQueryResult>>>
{
    private readonly IPaymentGatewayProfileQueryModelRepository _paymentGatewayProfileQueryModelRepository;

    public GetAllPaymentGatewaysQueryHandler(IPaymentGatewayProfileQueryModelRepository paymentGatewayProfileQueryModelRepository)
    {
        _paymentGatewayProfileQueryModelRepository = paymentGatewayProfileQueryModelRepository;
    }

    public async Task<Result<PagedQueryResult<GetAllPaymentGatewaysQueryResult>>> HandleAsync(GetAllPaymentGatewaysQuery query, CancellationToken cancellationToken)
    {
        return await _paymentGatewayProfileQueryModelRepository.GetAll(query, cancellationToken);
    }
}