using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class RepayReceiptRequestCommandHandler : ICommandHandler<RepayReceiptRequestCommand, Result<RepayReceiptRequestCommandResult>>
{
    private readonly ICreateReceiptRequestDomainService _createReceiptRequestDomainService;
    private readonly IOptions<PayMasterOptions> _payMasterOptions;

    public RepayReceiptRequestCommandHandler(
        ICreateReceiptRequestDomainService createReceiptRequestDomainService,
        IOptions<PayMasterOptions> payMasterOptions)
    {
        _createReceiptRequestDomainService = createReceiptRequestDomainService;
        _payMasterOptions = payMasterOptions;
    }

    public async Task<Result<RepayReceiptRequestCommandResult>> HandleAsync(
        RepayReceiptRequestCommand command,
        CancellationToken cancellationToken)
    {
        var newReceiptRequest = await _createReceiptRequestDomainService.RepayAsync(command.ReceiptRequestId, command.GatewayProviderCode,
            command.GatewayProviderId, _payMasterOptions.Value.DefaultGatewayProviderCode, cancellationToken);

        return new RepayReceiptRequestCommandResult
        {
            Id = newReceiptRequest.Id.ToString(CultureInfo.InvariantCulture),
        };
    }
}