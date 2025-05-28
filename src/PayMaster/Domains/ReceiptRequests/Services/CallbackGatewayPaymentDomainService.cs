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

    public async Task<CallBackResult> ProcessCallBackAsync(long? gatewayPaymentId, long gatewayProviderId, string callBackData)
    {
        ReceiptRequest? receiptRequest = null;
        ReceiptRequestGatewayPayment? gatewayPayment = null;
        IPaymentGatewayProvider? paymentGatewayProvider = null;

        var gatewayProvider = await _gatewayProviderRepository.GetAsync(c => c.Id == gatewayProviderId);

        if (gatewayProvider == null)
        {
            throw new GatewayProviderNotFoundException();
        }

        paymentGatewayProvider = _gatewayProviderFactory.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

        if (paymentGatewayProvider == null)
        {
            throw new GatewayProviderCreationException();
        }

        var extractCallBackDataResult = paymentGatewayProvider.ExtractCallBackData(callBackData);

        if (gatewayPaymentId.HasValue)
        {
            receiptRequest = await _receiptRequestRepository.GetByGatewayPaymentIDAsync(gatewayPaymentId.Value);

            if (receiptRequest is null)
            {
                throw new InvalidPaymentException();
            }

            gatewayPayment = receiptRequest.GetGatewayPayment(gatewayPaymentId.Value);

            if (gatewayPayment is null)
            {
                throw new InvalidPaymentException();
            }

            if (gatewayPayment.GatewayProviderId != gatewayProviderId)
            {
                throw new InvalidPaymentException();
            }
        }
        else
        {
            if (extractCallBackDataResult.UniqueRequestId.HasValue)
            {
                receiptRequest = await _receiptRequestRepository.GetByGatewayPaymentIDAsync(
                    extractCallBackDataResult.UniqueRequestId.Value);

                gatewayPaymentId = extractCallBackDataResult.UniqueRequestId;
            }
            else if (!string.IsNullOrEmpty(extractCallBackDataResult.CreateReference))
            {
                receiptRequest = await _receiptRequestRepository.GetByGatewayPaymentCreateReferenceAsync(
                    extractCallBackDataResult.CreateReference, gatewayProviderId);

                gatewayPaymentId = receiptRequest?.GatewayPayments
                    .FirstOrDefault(c => c.CreateReference == extractCallBackDataResult.CreateReference)?.Id;
            }
            else
            {
                throw new BusinessException("EmptyCallbackDataException");
            }

            if (receiptRequest is null || !gatewayPaymentId.HasValue)
            {
                throw new InvalidPaymentException();
            }

            gatewayPayment = receiptRequest.GetGatewayPayment(gatewayPaymentId.Value);

            if (gatewayPayment is null)
            {
                throw new InvalidPaymentException();
            }
        }

        var verifyResult = await receiptRequest.StartCallBackForGatewayPayment(
            gatewayPayment,
            paymentGatewayProvider,
            _clock,
            extractCallBackDataResult);

        return new CallBackResult(receiptRequest, gatewayPayment, verifyResult);
    }
}

