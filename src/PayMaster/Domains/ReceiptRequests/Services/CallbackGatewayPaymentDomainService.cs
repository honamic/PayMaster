using Honamic.Framework.Domain;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;
using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Services;

public class CallbackGatewayPaymentDomainService : ICallbackGatewayPaymentDomainService
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IPaymentGatewayProviderRepository _gatewayProviderRepository;
    private readonly IPaymentGatewayProviderFactory _gatewayProviderFactory;
    private readonly IClock _clock;
    public CallbackGatewayPaymentDomainService(
        IReceiptRequestRepository receiptRequestRepository,
        IPaymentGatewayProviderRepository gatewayProviderRepository,
        IPaymentGatewayProviderFactory gatewayProviderFactory,
        IClock clock)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _gatewayProviderRepository = gatewayProviderRepository;
        _gatewayProviderFactory = gatewayProviderFactory;
        _clock = clock;
    }

    public async Task<CallBackResult> ProcessCallBackAsync(long receiptRequestId, long gatewayPaymentId, string callbackData)
    {
        ReceiptRequest? receiptRequest = await _receiptRequestRepository
            .GetAsync(c=>c.Id== receiptRequestId);

        if (receiptRequest is null)
        {
            throw new InvalidPaymentException();
        }

        var gatewayPayment = receiptRequest.GetGatewayPayment(gatewayPaymentId);

        if (gatewayPayment is null)
        {
            throw new InvalidPaymentException();
        }

        if (!gatewayPayment.CanProcessCallback())
        {
            throw new PaymentStatusNotValidForProcessingException();
        }

        var gatewayProvider = await _gatewayProviderRepository
                .GetAsync(c => c.Id == gatewayPayment.GatewayProviderId);

        if (gatewayProvider == null)
        {
            throw new GatewayProviderNotFoundException();
        }

        var paymentGatewayProvider = _gatewayProviderFactory
                 .Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

        if (paymentGatewayProvider == null)
        {
            throw new GatewayProviderCreationException();
        }

        var verifyResult = await receiptRequest.StartCallbackForGatewayPayment(
            gatewayPayment,
            paymentGatewayProvider,
            callbackData,
            _clock);

        return new CallBackResult(receiptRequest, gatewayPayment, verifyResult);
    }
}