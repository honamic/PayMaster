using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;

public class GetActivePaymentGatewaysQueryHandler : IQueryHandler<GetActivePaymentGatewaysQuery, Result<List<GetActivePaymentGatewaysQueryResult>>>
{
    private readonly IPaymentGatewayProfileQueryModelRepository _paymentGatewayProfileQueryModelRepository;

    public GetActivePaymentGatewaysQueryHandler(IPaymentGatewayProfileQueryModelRepository paymentGatewayProfileQueryModelRepository)
    {
        _paymentGatewayProfileQueryModelRepository = paymentGatewayProfileQueryModelRepository;
    }

    public async Task<Result<List<GetActivePaymentGatewaysQueryResult>>> HandleAsync(GetActivePaymentGatewaysQuery query, CancellationToken cancellationToken)
    {
        return await _paymentGatewayProfileQueryModelRepository.GetActivePaymentGatewayProfilesAsync(query, cancellationToken);
    }
}