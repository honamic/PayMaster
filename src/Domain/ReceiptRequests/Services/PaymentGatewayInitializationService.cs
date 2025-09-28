using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions; 
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;
using Microsoft.Extensions.Logging; 

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;

public class PaymentGatewayInitializationService : IPaymentGatewayInitializationService
{
    private readonly IPaymentGatewayProfileRepository _repository;
    private readonly IPaymentGatewayProviderFactory _factory;
    private readonly IClock _clock;
    private readonly ILogger<PaymentGatewayInitializationService> _logger;

    public PaymentGatewayInitializationService(
        IPaymentGatewayProfileRepository repository,
        IPaymentGatewayProviderFactory factory,
        IClock clock,
        ILogger<PaymentGatewayInitializationService> logger)
    {
        _repository = repository;
        _factory = factory;
        _clock = clock;
        _logger = logger; 
    }

    public async Task<PaymentInitializationResult> InitializePaymentAsync(ReceiptRequest receiptRequest, string CallBackUrl)
    {
        PaymentInitializationResult result = new PaymentInitializationResult();

        if (receiptRequest is null)
        {
            throw new ArgumentNullException(nameof(receiptRequest), "Receipt request cannot be null.");
        }

        receiptRequest.InitializeGatewayPayment();

        var gatewayPayment = receiptRequest.GetPayableGatewayPayment();

        if (gatewayPayment is null)
        {
            throw new PayableGatewayPaymentNotFoundException();
        }

        result.GatewayPayment = gatewayPayment;

        var gatewayProvider = await _repository.GetByIdAsync(gatewayPayment.PaymentGatewayProfileId);

        if (gatewayProvider == null)
        {
            throw new GatewayProviderNotFoundException();
        }

        var provider = _factory.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations);

        var callbackUrl = GetCallbackUrl(CallBackUrl, receiptRequest, gatewayPayment);

        ReceiptRequestTryLog tryLog = CreateTryLog(receiptRequest, gatewayPayment);

        var createResult = await provider.CreateAsync(new CreateRequest
        {
            Amount = gatewayPayment.Amount,
            Currency = gatewayPayment.Currency,
            UniqueRequestId = gatewayPayment.Id,
            CallbackUrl = callbackUrl,
        });

        tryLog.SetSuccess(createResult.Success, createResult.LogData);

        if (createResult.Success)
        {
            gatewayPayment.SetWaitingStatus(
                createResult.CreateReference,
                createResult.StatusDescription,
                _clock.NowWithOffset);
        }
        else
        {
            gatewayPayment.SetFailedStatus(
                PaymentGatewayFailedReason.CreateFailed,
                createResult.StatusDescription);
        }

        ApplyCreateResultToInitializationResult(result, createResult);

        return result;
    }

    private string GetCallbackUrl(string callBackUrl, ReceiptRequest receiptRequest, ReceiptRequestGatewayPayment gatewayPayment)
    {
        return callBackUrl
                    .Replace(Constants.Parameters.ReceiptRequestIdParameter, receiptRequest.Id.ToString())
                    .Replace(Constants.Parameters.GatewayPaymentIdParameter, gatewayPayment.Id.ToString());
    }

    private static ReceiptRequestTryLog CreateTryLog(ReceiptRequest receiptRequest, ReceiptRequestGatewayPayment gatewayPayment)
    {
        var tryLog = new ReceiptRequestTryLog()
        {
            CreateAt = DateTimeOffset.Now,
            TryType = ReceiptRequestTryLogType.CreatePaymentProvider,
            ReceiptRequestId = receiptRequest.Id,
            ReceiptRequestGatewayPaymentId = gatewayPayment.Id,
        };

        receiptRequest.TryLogs.Add(tryLog);

        return tryLog;
    }

    private static void ApplyCreateResultToInitializationResult(PaymentInitializationResult result, CreateResult createResult)
    {
        result.Success = createResult.Success;
        result.PayUrl = createResult.PayUrl;
        result.PayVerb = createResult.PayVerb;
        result.PayParams = createResult.PayParams;
    }
}