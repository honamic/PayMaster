
namespace Honamic.PayMaster.Options;

public class PayMasterOptions
{
    public PayMasterOptions()
    {
        DefaultIssuerCode = "Default";
        DefaultGatewayProviderCode = "Default";
        SupportedCurrencies = ["IRR", "USD"];
        CallBackUrl = "https://yoursite.com/PaymentMaster/callback/{ReceiptRequestId}/{GatewayPaymentId}/";
    }

    public string? DefaultIssuerCode { get; set; } = default!;
    public string? DefaultGatewayProviderCode { get; set; } = default!;
    public string[] SupportedCurrencies { get; set; } = default!;
    public string CallBackUrl { get; set; } = default!;
}
