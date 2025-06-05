using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Services;

// اینترفیس دامین سرویس
public interface ICallbackGatewayPaymentDomainService
{
    Task<CallbackResult> ProcessCallbackAsync(long receiptRequestId, long gatewayPaymentId, string callBackData);
}

public class CallbackResult
{
    public ReceiptRequest ReceiptRequest { get; }
    public ReceiptRequestGatewayPayment GatewayPayment { get; }
    public VerifyResult? VerifyResult { get; }

    public CallbackResult(
        ReceiptRequest receiptRequest,
        ReceiptRequestGatewayPayment gatewayPayment,
        VerifyResult? verifyResult)
    {
        ReceiptRequest = receiptRequest;
        GatewayPayment = gatewayPayment;
        VerifyResult = verifyResult;
    }
}
