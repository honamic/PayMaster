using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;
using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.CommandHandlers;

public class UpdatePaymentGatewayProfileCommandHandler : ICommandHandler<UpdatePaymentGatewayProfileCommand, Result<UpdatePaymentGatewayProfileCommandResult>>
{
    private readonly IPaymentGatewayProviderFactory _paymentGatewayProviderFactory;
    private readonly IPaymentGatewayProfileRepository _paymentGatewayProfileRepository;

    public UpdatePaymentGatewayProfileCommandHandler(IPaymentGatewayProviderFactory paymentGatewayProviderFactory, IPaymentGatewayProfileRepository paymentGatewayProfileRepository)
    {
        _paymentGatewayProviderFactory = paymentGatewayProviderFactory;
        _paymentGatewayProfileRepository = paymentGatewayProfileRepository;
    }

    public async Task<Result<UpdatePaymentGatewayProfileCommandResult>> HandleAsync(UpdatePaymentGatewayProfileCommand command, CancellationToken cancellationToken)
    {
        var paymentGatewayProfile = await _paymentGatewayProfileRepository.GetByIdAsync(command.Id);

        if (paymentGatewayProfile == null)
        {
            return Result<UpdatePaymentGatewayProfileCommandResult>.Failure(ResultStatus.NotFound, "شناسه یافت نشد.");
        }

        if (!string.IsNullOrEmpty(command.ProviderType) && paymentGatewayProfile.ProviderType != command.ProviderType)
        {
            return Result<UpdatePaymentGatewayProfileCommandResult>.Failure(ResultStatus.ValidationFailed, "تغییر نوع درگاه امکان پذیر نیست.");
        }

        if (paymentGatewayProfile.Code != command.Code)
        {
            var exist = await _paymentGatewayProfileRepository.ExistsByCodeAsync(command.Code.Trim(), null);

            if (exist)
            {
                return Result<UpdatePaymentGatewayProfileCommandResult>.Failure(ResultStatus.ValidationFailed, "این کد قبلا استفاده شده است.");
            }
        }

        var updateParameters = new UpdatePaymentGatewayProfileParameters
        {
            Code = command.Code.Trim(),
            JsonConfigurations = command.JsonConfigurations,
            Title = command.Title,
            Enabled = command.Enabled,
            LogoPath = command.LogoPath,
            MaximumAmount = command.MaximumAmount,
            MinimumAmount = command.MinimumAmount,
        };

        paymentGatewayProfile.UpdateAndValidate(updateParameters, _paymentGatewayProviderFactory);

        return Result<UpdatePaymentGatewayProfileCommandResult>.Success();
    }
}