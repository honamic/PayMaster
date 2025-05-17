using Honamic.Framework.Domain;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Core.ReceiptRequests;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;

namespace Honamic.PayMaster.Core.Services;

public class PaymentProcessingService : IPaymentProcessingService
{
    private readonly IPaymentGatewayProviderRepository _repository;
    private readonly IPaymentGatewayProviderFactory _factory;
    private readonly IClock _clock;
    private readonly ILogger<PaymentProcessingService> _logger;

    public PaymentProcessingService(
        IPaymentGatewayProviderRepository repository,
        IPaymentGatewayProviderFactory factory,
        IClock clock,
        ILogger<PaymentProcessingService> logger)
    {
        _repository = repository;
        _factory = factory;
        _clock = clock;
        _logger = logger;
    }


    public async Task<CreateResult> PreparePaymentAsync(
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

        var provider = _factory.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

        if (provider == null)
        {
            throw new InvalidOperationException("درگاه پرداخت ساخته نشد.");
        }

        var createResult = await provider.CreateAsync(new CreateRequest
        {
            Amount = gatewayPayment.Amount,
            Currency = gatewayPayment.Currency,
            UniqueRequestId = gatewayPayment.Id,
            CallbackUrl = callbackUrl,
        });


        if (createResult.Success)
        {
            gatewayPayment.SetWaitingStatus(
                createResult.CreateReference,
                createResult.Error,
                _clock.NowWithOffset);
        }
        else
        {
            gatewayPayment.SetFailedStatus(
                Enums.PaymentGatewayFailedReason.CreateFailed,
                createResult.Error);
        }

        return createResult;
    }
}
