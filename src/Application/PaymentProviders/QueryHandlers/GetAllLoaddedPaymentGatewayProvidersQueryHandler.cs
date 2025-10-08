using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProviders.Queries;
using Honamic.PayMaster.Extensions;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;

public class GetAllLoaddedPaymentGatewayProvidersQueryHandler : IQueryHandler<GetAllLoaddedPaymentGatewayProvidersQuery, Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>>
{
    public Task<Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>> HandleAsync(GetAllLoaddedPaymentGatewayProvidersQuery filter, CancellationToken cancellationToken)
    {
        var result = GatewayPaymentProviderServiceCollectionExtensions.Providers.Select(c => new GetAllLoaddedPaymentGatewayProvidersQueryResult
        {
            ProviderType = c.Key,
            DisplayName = c.Value,
        }).ToList();

        return Task.FromResult(Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>.Success(result));
    }
}