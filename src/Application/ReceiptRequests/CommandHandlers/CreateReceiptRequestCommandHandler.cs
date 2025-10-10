using Honamic.Framework.Application.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests.Services; 
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class CreateReceiptRequestCommandHandler : ICommandHandler<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>
{
    private readonly ICreateReceiptRequestDomainService _createReceiptRequestDomainService;
    private readonly IOptions<PayMasterOptions> _payMasterOptions;

    public CreateReceiptRequestCommandHandler(
        ICreateReceiptRequestDomainService createReceiptRequestDomainService,
        IOptions<PayMasterOptions> payMasterOptions)
    {
        _createReceiptRequestDomainService = createReceiptRequestDomainService;
        _payMasterOptions = payMasterOptions;
    }

    public async Task<Result<CreateReceiptRequestCommandResult>> HandleAsync(
        CreateReceiptRequestCommand command,
        CancellationToken cancellationToken)
    {
        var createParams = new CreateReceiptRequestParameters
        {
            Amount = command.Amount,
            Currency = command.Currency,
            PartyId = command.PartyId,
            PartyReference = command.PartyReference,
            IssuerReference = command.IssuerReference,
            NationalityCode = command.NationalityCode,
            Email = command.Email,
            Mobile = command.Mobile,
            IsLegal = command.IsLegal,
            Description = command.Description,
            AdditionalData = command.AdditionalData,

            IssuerCode = command.IssuerCode,
            GatewayProviderId=command.GatewayProviderId,
            GatewayProviderCode = command.GatewayProviderCode,

            SupportedCurrencies = _payMasterOptions.Value.SupportedCurrencies,
            DefaultGatewayProviderCode = _payMasterOptions.Value.DefaultGatewayProviderCode,
            DefaultIssuerCode = _payMasterOptions.Value.DefaultIssuerCode,
        };

        var newReceiptRequest = await _createReceiptRequestDomainService.CreateAsync(createParams);

        return new CreateReceiptRequestCommandResult
        {
            Id = newReceiptRequest.Id.ToString(CultureInfo.InvariantCulture),
        };
    }
}