using Honamic.Framework.Applications.Results;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;

namespace Honamic.PayMaster.Wrapper;

public interface IPayMasterFacade
{
    Task<Result<CreateReceiptRequestCommandResult>> CreateReceiptRequest(CreateReceiptRequestCommand model, CancellationToken cancellationToken);
    Task<Result<PayReceiptRequestCommandResult>> PayReceiptRequest(PayReceiptRequestCommand paycommand, CancellationToken cancellationToken);
}
