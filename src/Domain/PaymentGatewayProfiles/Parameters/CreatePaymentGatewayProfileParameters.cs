namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;

public class CreatePaymentGatewayProfileParameters
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public required string Code { get; set; }

    public required string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    public required string JsonConfigurations { get; set; }

    public bool Enabled { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }
}
