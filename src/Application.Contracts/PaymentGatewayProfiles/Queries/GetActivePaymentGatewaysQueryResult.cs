namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;

public class GetActivePaymentGatewaysQueryResult
{
    public long Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string? LogoPath { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }
}