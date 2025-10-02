using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;

public class CreateReceiptRequestDomainService : ICreateReceiptRequestDomainService
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IReceiptIssuerRepository _receiptIssuerRepository;
    private readonly IPaymentGatewayProfileRepository _paymentGatewayProfileRepository;
    private readonly IIdGenerator _idGenerator;
    public CreateReceiptRequestDomainService(
        IIdGenerator idGenerator,
        IReceiptRequestRepository receiptRequestRepository,
        IReceiptIssuerRepository receiptIssuerRepository,
        IPaymentGatewayProfileRepository paymentGatewayProfileRepository)
    {
        _idGenerator = idGenerator;
        _receiptRequestRepository = receiptRequestRepository;
        _receiptIssuerRepository = receiptIssuerRepository;
        _paymentGatewayProfileRepository = paymentGatewayProfileRepository;
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

    private async Task<PaymentGatewayProfile> GetPaymentGatewayProvider
            (long? gatewayProfileId, string? gatewayProfileCode, string? defaultGatewayProviderCode)
    {
        PaymentGatewayProfile? paymentGatewayProfile = null;

        if (!gatewayProfileId.HasValue
                   && string.IsNullOrEmpty(gatewayProfileCode)
                    && string.IsNullOrEmpty(defaultGatewayProviderCode))
        {
            throw new NoDefaultGatewayProviderException();
        }

        if (gatewayProfileId.HasValue)
        {
            paymentGatewayProfile = await _paymentGatewayProfileRepository.GetByIdAsync(gatewayProfileId.Value);
        }

        if (paymentGatewayProfile is null &&
            !string.IsNullOrEmpty(gatewayProfileCode))
        {
            paymentGatewayProfile = await _paymentGatewayProfileRepository.GetByCodeAsync(gatewayProfileCode);
        }

        if (!gatewayProfileId.HasValue
               && string.IsNullOrEmpty(gatewayProfileCode)
               && !string.IsNullOrEmpty(defaultGatewayProviderCode))
        {
            paymentGatewayProfile = await _paymentGatewayProfileRepository.GetByCodeAsync(defaultGatewayProviderCode);
        }

        if (paymentGatewayProfile is null)
        {
            throw new SpecifiedGatewayProviderNotFoundException();
        }

        return paymentGatewayProfile;
    }

    public async Task<ReceiptRequest> RepayAsync(long receiptRequestId, string? gatewayProviderCode, long? gatewayProviderId,
        string? defaultGatewayProviderCode, CancellationToken cancellationToken)
    {
        var receiptRequest = await _receiptRequestRepository.GetByIdAsync(receiptRequestId, cancellationToken);

        if (receiptRequest is null)
        {
            throw new InvalidPaymentException();
        }

        var paymentGatewayProvider = await GetPaymentGatewayProvider(gatewayProviderId,
            gatewayProviderCode, defaultGatewayProviderCode);

        var gatewayProvider = new ReceiptRequestGatewayProviderParameters
        {
            Id = paymentGatewayProvider.Id,
            Enabled = paymentGatewayProvider.Enabled,
            MinimumAmount = paymentGatewayProvider.MinimumAmount,
            MaximumAmount = paymentGatewayProvider.MaximumAmount,
        };

        receiptRequest.AddNewGatewayPayment(gatewayProvider, _idGenerator);

        return receiptRequest;
    }
}