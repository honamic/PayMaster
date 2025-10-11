
namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;

public interface ICallbackGatewayPaymentDomainService
{
    Task<CallbackResult> ProcessCallbackAsync(long gatewayPaymentId, string callBackData);
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
