using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;

public class CreateReceiptRequestDomainService : ICreateReceiptRequestDomainService
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IReceiptIssuerRepository _receiptIssuerRepository;
    private readonly IPaymentGatewayProviderRepository _paymentGatewayProviderRepository;
    private readonly IIdGenerator _idGenerator;
    public CreateReceiptRequestDomainService(
        IIdGenerator idGenerator,
        IReceiptRequestRepository receiptRequestRepository,
        IReceiptIssuerRepository receiptIssuerRepository,
        IPaymentGatewayProviderRepository paymentGatewayProviderRepository)
    {
        _idGenerator = idGenerator;
        _receiptRequestRepository = receiptRequestRepository;
        _receiptIssuerRepository = receiptIssuerRepository;
        _paymentGatewayProviderRepository = paymentGatewayProviderRepository;
    }

    public async Task<ReceiptRequest> CreateAsync(CreateReceiptRequestParameters createParams)
    {
        var receiptIssuer = await GetReceiptIssuer(createParams.IssuerCode, createParams.DefaultIssuerCode);

        var paymentGatewayProvider = await GetPaymentGatewayProvider
            (createParams.GatewayProviderId, createParams.GatewayProviderCode, createParams.DefaultGatewayProviderCode);

        createParams.Issuer = new ReceiptRequestIssuerParameters
        {
            Id = receiptIssuer.Id,
            Enabled = receiptIssuer.Enabled,
        };

        createParams.GatewayProvider = new ReceiptRequestGatewayProviderParameters
        {
            Id = paymentGatewayProvider.Id,
            Enabled = paymentGatewayProvider.Enabled,
            MinimumAmount = paymentGatewayProvider.MinimumAmount,
            MaximumAmount = paymentGatewayProvider.MaximumAmount,
        };

        var newReceiptRequest = ReceiptRequest.Create(createParams, _idGenerator);

        await _receiptRequestRepository.InsertAsync(newReceiptRequest);

        return newReceiptRequest;
    }


    private async Task<ReceiptIssuer> GetReceiptIssuer(string? issuerCode, string? defaultIssuerCode)
    {
        ReceiptIssuer? receiptIssuer = null;

        if (string.IsNullOrEmpty(issuerCode) && string.IsNullOrEmpty(defaultIssuerCode))
        {
            throw new NoIssuerSpecifiedException();
        }

        if (!string.IsNullOrEmpty(issuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository.GetByCodeAsync(issuerCode);
        }
        else if (!string.IsNullOrEmpty(defaultIssuerCode))
        {
            receiptIssuer = await _receiptIssuerRepository.GetByCodeAsync(defaultIssuerCode);
        }

        if (receiptIssuer is null)
        {
            throw new IssuerCodeNotFoundException(issuerCode);
        }

        return receiptIssuer;
    }

    private async Task<PaymentGatewayProvider> GetPaymentGatewayProvider
            (long? gatewayProviderId, string? gatewayProviderCode, string? defaultGatewayProviderCode)
    {
        PaymentGatewayProvider? paymentGatewayProvider = null;

        if (!gatewayProviderId.HasValue
                   && string.IsNullOrEmpty(gatewayProviderCode)
                    && string.IsNullOrEmpty(defaultGatewayProviderCode))
        {
            throw new NoDefaultGatewayProviderException();
        }

        if (gatewayProviderId.HasValue)
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository.GetByIdAsync(gatewayProviderId.Value);
        }

        if (paymentGatewayProvider is null &&
            !string.IsNullOrEmpty(gatewayProviderCode))
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository.GetByCodeAsync(gatewayProviderCode);
        }

        if (!gatewayProviderId.HasValue
               && string.IsNullOrEmpty(gatewayProviderCode)
               && !string.IsNullOrEmpty(defaultGatewayProviderCode))
        {
            paymentGatewayProvider = await _paymentGatewayProviderRepository.GetByCodeAsync(defaultGatewayProviderCode);
        }

        if (paymentGatewayProvider is null)
        {
            throw new SpecifiedGatewayProviderNotFoundException();
        }

        return paymentGatewayProvider;
    }
}