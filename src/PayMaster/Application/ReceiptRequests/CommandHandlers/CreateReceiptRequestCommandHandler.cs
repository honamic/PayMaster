using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.Options;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptIssuers;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class CreateReceiptRequestCommandHandler : ICommandHandler<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IReceiptIssuerRepository _receiptIssuerRepository;
    private readonly IPaymentGatewayProviderRepository _paymentGatewayProviderRepository;
    private readonly IIdGenerator _idGenerator;
    private readonly IOptions<PayMasterOptions> _payMasterOptions;

    public CreateReceiptRequestCommandHandler(IIdGenerator idGenerator,
        IReceiptRequestRepository receiptRequestRepository,
        IReceiptIssuerRepository receiptIssuerRepository,
        IPaymentGatewayProviderRepository paymentGatewayProviderRepository,
        IOptions<PayMasterOptions> payMasterOptions)
    {
        _idGenerator = idGenerator;
        _receiptRequestRepository = receiptRequestRepository;
        _receiptIssuerRepository = receiptIssuerRepository;
        _paymentGatewayProviderRepository = paymentGatewayProviderRepository;
        _payMasterOptions = payMasterOptions;
    }

    public async Task<Result<CreateReceiptRequestCommandResult>> HandleAsync(CreateReceiptRequestCommand command, CancellationToken cancellationToken)
    {
        ReceiptIssuer receiptIssuer = await GetReceiptIssuer(command);

        PaymentGatewayProvider paymentGatewayProvider = await GetPaymentGatewayProvider(command);

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

            Issuer = new ReceiptRequestIssuerParameters
            {
                Id = receiptIssuer.Id,
                Enabled = receiptIssuer.Enabled,
            },
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = paymentGatewayProvider.Id,
                Enabled = paymentGatewayProvider.Enabled,
                MinimumAmount = paymentGatewayProvider.MinimumAmount,
                MaximumAmount = paymentGatewayProvider.MaximumAmount,
            },

            SupportedCurrencies = _payMasterOptions.Value.SupportedCurrencies,
        };

        var newReceiptRequest = ReceiptRequest.Create(createParams, _idGenerator);

        await _receiptRequestRepository.InsertAsync(newReceiptRequest);

        return new CreateReceiptRequestCommandResult
        {
            Id = newReceiptRequest.Id.ToString(CultureInfo.InvariantCulture),
        };
    }

    private async Task<ReceiptIssuer> GetReceiptIssuer(CreateReceiptRequestCommand command)
    {
        ReceiptIssuer? receiptIssuer = null;

        if (string.IsNullOrEmpty(command.IssuerCode)
                && string.IsNullOrEmpty(_payMasterOptions.Value.DefaultIssuerCode))
        {
            throw new NoIssuerSpecifiedException();
        }

        if (!string.IsNullOrEmpty(command.IssuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository
               .GetAsync(c => c.Code == command.IssuerCode);
        }
        else if (!string.IsNullOrEmpty(_payMasterOptions.Value.DefaultIssuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository
                           .GetAsync(c => c.Code == _payMasterOptions.Value.DefaultIssuerCode);
        }

        if (receiptIssuer is null)
        {
            throw new IssuerCodeNotFoundException(command.IssuerCode);
        }

        return receiptIssuer;
    }

    private async Task<PaymentGatewayProvider> GetPaymentGatewayProvider(CreateReceiptRequestCommand command)
    {
        PaymentGatewayProvider? paymentGatewayProvider = null;
        if (!command.GatewayProviderId.HasValue
                   && string.IsNullOrEmpty(command.GatewayProviderCode)
                    && string.IsNullOrEmpty(_payMasterOptions.Value.DefaultGatewayProviderCode))
        {
            throw new NoDefaultGatewayProviderException();
        }

        if (command.GatewayProviderId.HasValue)
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository.GetAsync(c => c.Id == command.GatewayProviderId);
        }

        if (paymentGatewayProvider is null &&
            !string.IsNullOrEmpty(command.GatewayProviderCode))
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository.GetAsync(c => c.Code == command.GatewayProviderCode);
        }

        if (!command.GatewayProviderId.HasValue
               && string.IsNullOrEmpty(command.GatewayProviderCode))
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository
                .GetAsync(c => c.Code == _payMasterOptions.Value.DefaultGatewayProviderCode);
        }

        if (paymentGatewayProvider is null)
        {
            throw new SpecifiedGatewayProviderNotFoundException();
        }

        return paymentGatewayProvider;
    }
}