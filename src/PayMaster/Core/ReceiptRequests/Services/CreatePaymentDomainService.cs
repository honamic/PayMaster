using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;

namespace Honamic.PayMaster.Core.ReceiptRequests.Services;

public class CreatePaymentDomainService : ICreatePaymentDomainService
{
    private readonly IPaymentGatewayProviderRepository _repository;
    private readonly IPaymentGatewayProviderFactory _factory;
    private readonly IClock _clock;
    private readonly ILogger<CreatePaymentDomainService> _logger;

    public CreatePaymentDomainService(
        IPaymentGatewayProviderRepository repository,
        IPaymentGatewayProviderFactory factory,
        IClock clock,
        ILogger<CreatePaymentDomainService> logger)
    {
        _repository = repository;
        _factory = factory;
        _clock = clock;
        _logger = logger;
    }


    public async Task<CreateResult> CreatePaymentAsync(
        ReceiptRequest receiptRequest,
        string callbackUrl)
    {
        var gatewayPayment = receiptRequest.GetPayableGatewayPayment();

        if (gatewayPayment is null)
        {
            throw new InvalidOperationException("پرداخت درگاهی آماده پرداخت وجود ندارد.");
        }

        var gatewayProvider = await _repository
            .GetAsync(c => c.Id == gatewayPayment.GatewayProviderId);

        if (gatewayProvider == null)
        {
            throw new InvalidOperationException("درگاه پرداخت شناسایی نشد.");
        }

        var provider = _factory.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

        if (provider == null)
        {
            throw new InvalidOperationException("درگاه پرداخت ساخته نشد.");
        }

        callbackUrl = callbackUrl
            .Replace(Constants.GatewayPaymentIdParameter, gatewayPayment.Id.ToString())
            .Replace(Constants.GatewayProviderIdParameter, gatewayPayment.GatewayProviderId.ToString());

        var createResult = await receiptRequest
            .CreatePaymentByGatewayProviderAsync(gatewayPayment, provider, _clock, callbackUrl);

        return createResult;
    }
}
