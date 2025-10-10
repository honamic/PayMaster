using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.PaymentGatewayProviders.Queries;

public class GetAllLoaddedPaymentGatewayProvidersQuery : IQuery<Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>>
{

}


public class GetAllLoaddedPaymentGatewayProvidersQueryResult
{
    public string ProviderType { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}
