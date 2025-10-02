namespace Honamic.PayMaster.PaymentProvider.Sandbox.Models;

public class SanboxRequestDataModel
{
    public required string PayId { get; set; }

    public required decimal Amount { get; set; }
    
    public required string Currency { get; set; }

    public required string Token { get; set; }

    public required string CallbackUrl { get;  set; }

    public required string UniqueRequestId { get;  set; }

    public string? GatewayNote { get; set; }
    
    public string? MerchantName { get; set; }
}