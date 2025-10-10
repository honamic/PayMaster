using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;

public class GetPaymentGatewayQueryHandler : IQueryHandler<GetPaymentGatewayQuery, Result<GetPaymentGatewayQueryResult>>
{
    private readonly IPaymentGatewayProfileQueryModelRepository _paymentGatewayProfileQueryModelRepository;

    public GetPaymentGatewayQueryHandler(IPaymentGatewayProfileQueryModelRepository paymentGatewayProfileQueryModelRepository)
    {
        _paymentGatewayProfileQueryModelRepository = paymentGatewayProfileQueryModelRepository;
    }

 
    public async Task<Result<GetPaymentGatewayQueryResult?>> HandleAsync(GetPaymentGatewayQuery query, CancellationToken cancellationToken)
    {
        return await _paymentGatewayProfileQueryModelRepository.Get(query, cancellationToken);
    }
}