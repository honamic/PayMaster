using Honamic.Framework.Commands;
using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Core.ReceiptIssuers;
using Honamic.PayMaster.Core.ReceiptRequests;
using Honamic.PayMaster.Core.ReceiptRequests.Parameters;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class CreateReceiptRequestCommandHandler : ICommandHandler<CreateReceiptRequestCommand, CreateReceiptRequestCommandResult>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IReceiptIssuerRepository _receiptIssuerRepository;
    private readonly IPaymentGatewayProviderRepository _paymentGatewayProviderRepository;
    private readonly IIdGenerator _idGenerator;

    public CreateReceiptRequestCommandHandler(IIdGenerator idGenerator,
        IReceiptRequestRepository receiptRequestRepository,
        IReceiptIssuerRepository receiptIssuerRepository,
        IPaymentGatewayProviderRepository paymentGatewayProviderRepository)
    {
        _idGenerator = idGenerator;
        _receiptRequestRepository = receiptRequestRepository;
        _receiptIssuerRepository = receiptIssuerRepository;
        _paymentGatewayProviderRepository = paymentGatewayProviderRepository;
    }

    public async Task<CreateReceiptRequestCommandResult> HandleAsync(CreateReceiptRequestCommand command, CancellationToken cancellationToken)
    {
        string? defaultIsssuerCode = "Default";
        string? defaultGatewayProviderCode = "Default";

        string[] supportedCurrenciesOption = ["IRR", "USD"];

        ReceiptIssuer receiptIssuer = await GetReceiptIssuer(command, defaultIsssuerCode);

        PaymentGatewayProvider paymentGatewayProvider = await GetPaymentGatewayProvider(command, defaultGatewayProviderCode);

        var createParams = new CreateReceiptRequestParameters
        {
            Amount = command.Amount,
            Currency = command.Currency,
            PartyId = command.PartyId,
            PartyIdentity = command.PartyIdentity,
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

            SupportedCurrencies = supportedCurrenciesOption
        };

        var newReceiptRequest = ReceiptRequest.Create(createParams, _idGenerator);

        await _receiptRequestRepository.InsertAsync(newReceiptRequest);

        return new CreateReceiptRequestCommandResult
        {
            Id = newReceiptRequest.Id.ToString(CultureInfo.InvariantCulture),
        };
    }

    private async Task<ReceiptIssuer> GetReceiptIssuer(CreateReceiptRequestCommand command, string defaultIsssuerCode)
    {
        ReceiptIssuer? receiptIssuer = null;

        if (string.IsNullOrEmpty(command.IssuerCode)
                && string.IsNullOrEmpty(defaultIsssuerCode))
        {
            throw new ArgumentException("صادر کننده فیش مشخص نشده است و پیش فرض هم مشخص نشده است.");
        }

        if (!string.IsNullOrEmpty(command.IssuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository
               .GetAsync(c => c.Code == command.IssuerCode);
        }
        else if (!string.IsNullOrEmpty(defaultIsssuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository
                           .GetAsync(c => c.Code == defaultIsssuerCode);
        }

        if (receiptIssuer is null)
        {
            throw new ArgumentException($"کد صادرکننده فیش وجود ندارد [{command.IssuerCode}]");
        }


        return receiptIssuer;
    }

    private async Task<PaymentGatewayProvider> GetPaymentGatewayProvider(CreateReceiptRequestCommand command, string defaultGatewayProviderCode)
    {
        PaymentGatewayProvider? paymentGatewayProvider = null;
        if (!command.GatewayProviderId.HasValue
                   && string.IsNullOrEmpty(command.GatewayProviderCode)
                    && string.IsNullOrEmpty(defaultGatewayProviderCode))
        {
            throw new ArgumentException("درگاه پرداخت مشخص نشده است و پیش فرض هم مشخص نشده است.");
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
                .GetAsync(c => c.Code == defaultGatewayProviderCode);
        }

        if (paymentGatewayProvider is null)
        {
            throw new ArgumentException($"درگاه پرداخت مشخص شده وجود ندارد.");
        }

        return paymentGatewayProvider;
    }
}