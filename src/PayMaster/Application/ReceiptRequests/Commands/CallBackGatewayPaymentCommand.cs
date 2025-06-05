using Honamic.Framework.Commands; 
using Honamic.Framework.Applications.Results;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;

public class CallBackGatewayPaymentCommand : ICommand<Result<CallBackGatewayPaymentCommandResult>>
{
    public required string CallBackData { get; set; }
    public required string ReceiptRequestId { get; set; }
    public required string GatewayPaymentId { get; set; }

    public long GetReceiptRequestIdAsLong()
    {
        return long.Parse(ReceiptRequestId);
    }

    public long GetGatewayPaymentIdAsLong()
    {
        return long.Parse(GatewayPaymentId);
    }
}

public class CallBackGatewayPaymentCommandResult
{
    public CallBackGatewayPaymentCommandResult()
    {
        GatewayPayments = [];
    }

    public ReceiptRequestStatus Status { get; set; }
    public string ReceiptRequestId { get; set; }
    public string? IssuerReference { get; set; }
    public string? PartyReference { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string? AdditionalData { get; set; }
    public object? Issuer { get; set; }

    public List<CallBackGatewayPaymentGatewayPaymentsCommandResult> GatewayPayments { get; set; }
}
public class CallBackGatewayPaymentGatewayPaymentsCommandResult
{
    public string Id { get; internal set; }
    public PaymentGatewayStatus Status { get; internal set; }
    public decimal Amount { get; internal set; }
    public string Currency { get; internal set; }
    public PaymentGatewayFailedReason FailedReason { get; internal set; }
    public string? StatusDescription { get; internal set; }
    public string? FailedReasonName { get; internal set; }
}