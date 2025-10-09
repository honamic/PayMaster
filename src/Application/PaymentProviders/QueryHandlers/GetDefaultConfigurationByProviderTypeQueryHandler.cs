using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProviders.Queries;
using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;

public class GetDefaultConfigurationByProviderTypeQueryHandler :
    IQueryHandler<GetDefaultConfigurationByProviderTypeQuery, Result<GetDefaultConfigurationByProviderTypeQueryResult>>
{
    private readonly IPaymentGatewayProviderFactory _paymentGatewayProviderFactory;

    public GetDefaultConfigurationByProviderTypeQueryHandler(IPaymentGatewayProviderFactory paymentGatewayProviderFactory)
    {
        _paymentGatewayProviderFactory = paymentGatewayProviderFactory;
    }
    public Task<Result<GetDefaultConfigurationByProviderTypeQueryResult>> HandleAsync(GetDefaultConfigurationByProviderTypeQuery query, CancellationToken cancellationToken)
    {
        var provider = _paymentGatewayProviderFactory.CreateByDefaultConfiguration(query.ProviderType);

        var result = new GetDefaultConfigurationByProviderTypeQueryResult
        {
            Configuration = provider.GetDefaultJsonConfigurations(query.SandBox),
            ProviderType = query.ProviderType,
        };

        return Task.FromResult(Result<GetDefaultConfigurationByProviderTypeQueryResult>.Success(result));
    }
}