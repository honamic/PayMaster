using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.CommandHandlers;

public class CreatePaymentGatewayProfileCommandHandler : ICommandHandler<CreatePaymentGatewayProfileCommand, Result<CreatePaymentGatewayProfileCommandResult>>
{
    public Task<Result<CreatePaymentGatewayProfileCommandResult>> HandleAsync(CreatePaymentGatewayProfileCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}