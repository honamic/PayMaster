using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;

namespace Honamic.PayMaster.Wrapper;

public class PayMasterFacade : IPayMasterFacade
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;

    public PayMasterFacade(ICommandBus commandBus, IQueryBus queryBus)
    {
        _commandBus = commandBus;
        _queryBus = queryBus;
    }

    public Task<Result<CreateReceiptRequestCommandResult>> CreateReceiptRequest(CreateReceiptRequestCommand model, CancellationToken cancellationToken = default)
    {
        return _commandBus.DispatchAsync<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>(model, cancellationToken);
    }

    public Task<Result<InitiatePayReceiptRequestCommandResult>> InitiatePayReceiptRequest(InitiatePayReceiptRequestCommand model, CancellationToken cancellationToken = default)
    {
        return _commandBus.DispatchAsync<InitiatePayReceiptRequestCommand, Result<InitiatePayReceiptRequestCommandResult>>(model, cancellationToken);
    }

    public Task<Result<GetPublicReceiptRequestQueryResult?>> GetPublicReceiptRequest(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken)
    {
        return _queryBus.Dispatch(query, cancellationToken);
    }

    public Task<Result<List<GetActivePaymentGatewaysQueryResult>>> GetActivePaymentGateways(CancellationToken cancellationToken)
    {
        var query = new GetActivePaymentGatewaysQuery();
        return _queryBus.Dispatch(query, cancellationToken);
    }
}