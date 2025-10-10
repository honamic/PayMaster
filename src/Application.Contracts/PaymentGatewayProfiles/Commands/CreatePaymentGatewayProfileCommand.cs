using Honamic.Framework.Application.Authorizes;
using Honamic.Framework.Application.Results;
using Honamic.Framework.Commands;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;

[DynamicPermission(
    DisplayName = "ایجاد درگاه پرداخت آنلاین",
    Group = "PaymentGateways",
    Module = PayMasterConstants.ModuleName,
    Name = null,
    Description = "")]

public class CreatePaymentGatewayProfileCommand : ICommand<Result<CreatePaymentGatewayProfileCommandResult>>
{
    public long? Id { get; set; }

    public required string Code { get; set; }

    public required string Title { get; set; }

    public required string ProviderType { get; set; }

    public required string JsonConfigurations { get; set; }

    public string? LogoPath { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public bool Enabled { get; set; }
}

public class CreatePaymentGatewayProfileCommandResult
{
    public long Id { get; set; }

}