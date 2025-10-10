using Honamic.Framework.Application.Results;
using Honamic.Framework.Commands;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;
public class RepayReceiptRequestCommand : ICommand<Result<RepayReceiptRequestCommandResult>>
{
    public long ReceiptRequestId { get; set; }
    public string? GatewayProviderCode { get; set; }
    public long? GatewayProviderId { get; set; }
}

public class RepayReceiptRequestCommandResult
{
    public string Id { get; set; } = default!;
}