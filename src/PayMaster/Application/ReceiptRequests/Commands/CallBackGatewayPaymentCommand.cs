using Honamic.Framework.Commands;
using Honamic.Framework.Applications.Authorizes;
using Honamic.Framework.Applications.Results;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;

[DynamicAuthorize]
public class CallBackGatewayPaymentCommand : ICommand<Result<CallBackGatewayPaymentCommandResult>>
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