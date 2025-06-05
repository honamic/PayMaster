using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Services;

public interface ICallbackGatewayPaymentDomainService
{
    Task<CallbackResult> ProcessCallbackAsync(long receiptRequestId, long gatewayPaymentId, string callBackData);
}

public class CallbackResult
{
    public ReceiptRequest ReceiptRequest { get; }
    public ReceiptRequestGatewayPayment GatewayPayment { get; }
    public string IssuerCallbackUrl { get; }

    public CallbackResult(
        ReceiptRequest receiptRequest,
        ReceiptRequestGatewayPayment gatewayPayment,
        string issuerCallbackUrl)
    {
        ReceiptRequest = receiptRequest;
        GatewayPayment = gatewayPayment;
        IssuerCallbackUrl = issuerCallbackUrl;
    }
}
