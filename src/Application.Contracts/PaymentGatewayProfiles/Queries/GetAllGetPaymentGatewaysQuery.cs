using Honamic.Framework.Applications.Authorizes;
using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
 
[DynamicPermission(
    DisplayName = "لیست درگاههای پرداخت آنلاین",
    Group = "PaymentGateways",
    Module = PayMasterConstants.ModuleName,
    Name = null,
    Description = "")]

public class GetAllPaymentGatewaysQuery : PagedQueryFilter, IQuery<Result<PagedQueryResult<GetAllPaymentGatewaysQueryResult>>>
{
    public GetAllPaymentGatewaysQuery()
    {
        OrderBy = "Id desc";
    }
}

public class GetAllPaymentGatewaysQueryResult
{
    public long Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string? LogoPath { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public string ProviderType { get; set; }

    public bool Enabled { get; set; }
}