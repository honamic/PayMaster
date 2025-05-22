using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.Options;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Honamic.PayMaster.Core.ReceiptRequests.Services;

public class CreatePaymentDomainService : ICreatePaymentDomainService
{
    private readonly IPaymentGatewayProviderRepository _repository;
    private readonly IPaymentGatewayProviderFactory _factory;
    private readonly IClock _clock;
    private readonly ILogger<CreatePaymentDomainService> _logger;
    private readonly IOptions<PayMasterOptions> _payMasterOptions;

    public CreatePaymentDomainService(
        IPaymentGatewayProviderRepository repository,
        IPaymentGatewayProviderFactory factory,
        IClock clock,
        ILogger<CreatePaymentDomainService> logger,
        IOptions<PayMasterOptions> payMasterOptions)
    {
        _repository = repository;
        _factory = factory;
        _clock = clock;
        _logger = logger;
        _payMasterOptions = payMasterOptions;
    }

    public async Task<CreateResult> CreatePaymentAsync(ReceiptRequest receiptRequest)
    {
        CreateResult createResult;

        var tryLog = new ReceiptRequestTryLog()
        {
            CreateAt = DateTimeOffset.Now,
            TryType = Enums.ReceiptRequestTryLogType.CreatePaymentProvider,
            ReceiptRequestId = receiptRequest.Id,
        };

        try
        {
            var gatewayPayment = receiptRequest.GetPayableGatewayPayment();

            if (gatewayPayment is null)
            {
                throw new InvalidOperationException("پرداخت درگاهی آماده پرداخت وجود ندارد.");
            }

            tryLog.ReceiptRequestGatewayPaymentId = gatewayPayment.Id;

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

            var callbackUrl = _payMasterOptions.Value.CallBackUrl
                .Replace(Constants.GatewayPaymentIdParameter, gatewayPayment.Id.ToString())
                .Replace(Constants.GatewayProviderIdParameter, gatewayPayment.GatewayProviderId.ToString());

            createResult = await receiptRequest
                .CreatePaymentByGatewayProviderAsync(gatewayPayment, provider, _clock, callbackUrl);

            tryLog.Success = createResult.Success;
            tryLog.Data = createResult.LogData;
        }
        catch (Exception ex)
        {
            tryLog.Data.SetException(ex);
            throw;
        }

        receiptRequest.TryLogs.Add(tryLog);

        return createResult;
    }
}
