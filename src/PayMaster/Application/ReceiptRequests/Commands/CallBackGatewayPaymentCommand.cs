using Honamic.Framework.Commands;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;
public class CallBackGatewayPaymentCommand : ICommand<CallBackGatewayPaymentCommandResult>
{
    public required string GatewayProviderId { get; set; }
    public required string CallBackData { get; set; }
    public string? GatewayPaymentId { get; set; }

    public long GetGatewayProviderIdAsLong()
    {
        return long.Parse(GatewayProviderId);
    }

    public long? GetGatewayPaymentIdAsLong()
    {
        if (!string.IsNullOrEmpty(GatewayPaymentId))
        {
            return long.Parse(GatewayPaymentId);
        }

        return null;
    }
}

public class CallBackGatewayPaymentCommandResult
{

}