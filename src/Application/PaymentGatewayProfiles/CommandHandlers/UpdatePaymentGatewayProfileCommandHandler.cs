using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.CommandHandlers;

public class UpdatePaymentGatewayProfileCommandHandler : ICommandHandler<UpdatePaymentGatewayProfileCommand, Result<UpdatePaymentGatewayProfileCommandResult>>
{
    public Task<Result<UpdatePaymentGatewayProfileCommandResult>> HandleAsync(UpdatePaymentGatewayProfileCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}