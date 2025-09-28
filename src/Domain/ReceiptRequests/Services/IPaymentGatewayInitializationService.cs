using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;
public interface IPaymentGatewayInitializationService
{
    Task<PaymentInitializationResult> InitializePaymentAsync(ReceiptRequest receiptRequest,string CallBackUrl);
}


public class PaymentInitializationResult
{
    public bool Success { get; set; }
        
    public ReceiptRequestGatewayPayment? GatewayPayment { get; set; }
 
    public string? PayUrl { get;  set; }
   
    public PayVerb? PayVerb { get;  set; }

    public Dictionary<string, string>? PayParams { get;  set; }
}