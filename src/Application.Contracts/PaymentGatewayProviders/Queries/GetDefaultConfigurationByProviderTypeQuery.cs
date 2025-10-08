using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.PaymentGatewayProviders.Queries;

public class GetDefaultConfigurationByProviderTypeQuery : IQuery<Result<GetDefaultConfigurationByProviderTypeQueryResult>>
{
    public required string ProviderType { get; set; }

    public bool SandBox { get; set; }
}


public class GetDefaultConfigurationByProviderTypeQueryResult
{
    public string ProviderType { get; set; } = default!;

    public string Configuration { get; set; } = default!;
}
