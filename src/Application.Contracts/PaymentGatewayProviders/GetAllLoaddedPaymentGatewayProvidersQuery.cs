using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.PaymentGatewayProviders;

public class GetAllLoaddedPaymentGatewayProvidersQuery : IQuery<Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>>
{

}


public class GetAllLoaddedPaymentGatewayProvidersQueryResult
{
    public string ProviderType { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}
