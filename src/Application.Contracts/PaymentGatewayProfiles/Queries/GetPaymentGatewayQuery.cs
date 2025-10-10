using Honamic.Framework.Application.Authorizes;
using Honamic.Framework.Application.Results; 
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;

[DynamicPermission(
    DisplayName = "نمایش درگاه پرداخت آنلاین",
    Group = "PaymentGateways",
    Module = PayMasterConstants.ModuleName,
    Name = null,
    Description = "")]

public class GetPaymentGatewayQuery : IQuery<Result<GetPaymentGatewayQueryResult>>
{
    public long Id { get; set; }
}

public class GetPaymentGatewayQueryResult
{
    public long Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string? LogoPath { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public string ProviderType { get; set; } = default!;

    public string JsonConfigurations { get; set; } = default!;

    public bool Enabled { get; set; }
}