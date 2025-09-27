using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;

namespace Honamic.PayMaster.Wrapper;

public class PayMasterFacade : IPayMasterFacade
{
    private readonly ICommandBus _commandBus;

    public PayMasterFacade(ICommandBus commandBus)
    {
        this._commandBus = commandBus;
    }

    public Task<Result<CreateReceiptRequestCommandResult>> CreateReceiptRequest(CreateReceiptRequestCommand model, CancellationToken cancellationToken = default)
    {
        return _commandBus.DispatchAsync<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>(model, cancellationToken);
    }

    public Task<Result<PayReceiptRequestCommandResult>> PayReceiptRequest(PayReceiptRequestCommand model, CancellationToken cancellationToken = default)
    {
        return _commandBus.DispatchAsync<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>(model, cancellationToken);
    }

}