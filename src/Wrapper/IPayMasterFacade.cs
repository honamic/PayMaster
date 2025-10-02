using Honamic.Framework.Applications.Results;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;

namespace Honamic.PayMaster.Wrapper;

public interface IPayMasterFacade
{
    Task<Result<CreateReceiptRequestCommandResult>> CreateReceiptRequest(CreateReceiptRequestCommand command, CancellationToken cancellationToken);
    Task<Result<InitiatePayReceiptRequestCommandResult>> InitiatePayReceiptRequest(InitiatePayReceiptRequestCommand command, CancellationToken cancellationToken);
    Task<Result<GetPublicReceiptRequestQueryResult?>> GetPublicReceiptRequest(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetActivePaymentGatewaysQueryResult>>> GetActivePaymentGateways(CancellationToken cancellationToken);
    Task<Result<RepayReceiptRequestCommandResult>> RepayReceiptRequest(RepayReceiptRequestCommand command, CancellationToken cancellationToken);
}
