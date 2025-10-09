namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;

public class UpdatePaymentGatewayProfileParameters
{
    public required string Title { get; set; }
    
    public required string Code { get; set; }
    
    public required string JsonConfigurations { get; set; }
    
    public string? LogoPath { get; set; }
    
    public bool Enabled { get; set; }
    
    public decimal? MinimumAmount { get; set; }
    
    public decimal? MaximumAmount { get; set; }
}