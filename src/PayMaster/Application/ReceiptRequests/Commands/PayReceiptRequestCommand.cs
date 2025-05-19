using Honamic.Framework.Commands;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;
public class PayReceiptRequestCommand : ICommand<PayReceiptRequestCommandResult>
{
    public required string ReceiptRequestId { get; set; }

    public long GetReceiptRequestIdAsLong()
    {
        return long.Parse(ReceiptRequestId);
    }
}

public class PayReceiptRequestCommandResult
{
    public required string ReceiptRequestId { get; set; }

    public string? ReceiptRequestGatewayPaymentId { get; set; }

    public Dictionary<string, string>? PayParams { get; set; }

    public string? PayUrl { get; set; }

    public PayVerb? PayVerb { get; set; }
}