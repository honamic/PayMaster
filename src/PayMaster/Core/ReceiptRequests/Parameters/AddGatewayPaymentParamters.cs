namespace Honamic.PayMaster.Core.ReceiptRequests.Parameters;
public class CreateGatewayPaymentParamters
{
    public required long Id { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required ReceiptRequestGatewayProviderParameters GatewayProvider { get; set; }
}
