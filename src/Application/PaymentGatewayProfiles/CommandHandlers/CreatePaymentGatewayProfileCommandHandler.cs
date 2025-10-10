using Honamic.Framework.Application.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;
using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.Application.PaymentGatewayProfiles.CommandHandlers;

public class CreatePaymentGatewayProfileCommandHandler : ICommandHandler<CreatePaymentGatewayProfileCommand, Result<CreatePaymentGatewayProfileCommandResult>>
{
    private readonly IIdGenerator _idGenerator;
    private readonly IPaymentGatewayProviderFactory _paymentGatewayProviderFactory;
    private readonly IPaymentGatewayProfileRepository _paymentGatewayProfileRepository;

    public CreatePaymentGatewayProfileCommandHandler(IPaymentGatewayProviderFactory paymentGatewayProviderFactory, IPaymentGatewayProfileRepository paymentGatewayProfileRepository, IIdGenerator idGenerator)
    {
        _paymentGatewayProviderFactory = paymentGatewayProviderFactory;
        _paymentGatewayProfileRepository = paymentGatewayProfileRepository;
        _idGenerator = idGenerator;
    }

    public async Task<Result<CreatePaymentGatewayProfileCommandResult>> HandleAsync(CreatePaymentGatewayProfileCommand command, CancellationToken cancellationToken)
    {
        var exist = await _paymentGatewayProfileRepository.ExistsByCodeAsync(command.Code.Trim(), null);

        if (exist)
        {
            return Result<CreatePaymentGatewayProfileCommandResult>.Failure(ResultStatus.ValidationFailed, "این کد قبلا استفاده شده است.");
        }

        var createParameters = new CreatePaymentGatewayProfileParameters
        {
            Id = _idGenerator.GetNewId(),
            Code = command.Code.Trim(),
            JsonConfigurations = command.JsonConfigurations,
            ProviderType = command.ProviderType,
            Title = command.Title,
            Enabled = command.Enabled,
            LogoPath = command.LogoPath,
            MaximumAmount = command.MaximumAmount,
            MinimumAmount = command.MinimumAmount,
        };

        var newPaymentGatewayProfile = PaymentGatewayProfile.CreateAndValidatProvider(createParameters, _paymentGatewayProviderFactory);

        await _paymentGatewayProfileRepository.InsertAsync(newPaymentGatewayProfile, cancellationToken);

        return new CreatePaymentGatewayProfileCommandResult { Id = newPaymentGatewayProfile.Id };
    }
}