using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;

public class CallbackGatewayPaymentDomainService : ICallbackGatewayPaymentDomainService
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IReceiptIssuerRepository _receiptIssuerRepository;
    private readonly IPaymentGatewayProfileRepository _gatewayProfileRepository;
    private readonly IPaymentGatewayProviderFactory _gatewayProviderFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    public CallbackGatewayPaymentDomainService(
        IReceiptRequestRepository receiptRequestRepository,
        IPaymentGatewayProfileRepository gatewayProfileRepository,
        IReceiptIssuerRepository receiptIssuerRepository,
        IPaymentGatewayProviderFactory gatewayProviderFactory,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _gatewayProfileRepository = gatewayProfileRepository;
        _receiptIssuerRepository = receiptIssuerRepository;
        _gatewayProviderFactory = gatewayProviderFactory;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<CallbackResult> ProcessCallbackAsync(long receiptRequestId, long gatewayPaymentId, string callbackData)
    {
        ReceiptRequest? receiptRequest = await _receiptRequestRepository.GetByIdAsync(receiptRequestId);

        if (receiptRequest is null)
        {
            throw new InvalidPaymentException();
        }

        receiptRequest.EnsureCanProcessCallback();

        var gatewayPayment = receiptRequest.GetGatewayPayment(gatewayPaymentId);

        if (gatewayPayment is null)
        {
            throw new InvalidPaymentException();
        }

        if (!gatewayPayment.CanProcessCallback())
        {
            throw new PaymentStatusNotValidForProcessingException();
        }

        var gatewayProvider = await _gatewayProfileRepository
                .GetByIdAsync(gatewayPayment.PaymentGatewayProfileId);

        if (gatewayProvider == null)
        {
            throw new GatewayProviderNotFoundException();
        }

        var paymentGatewayProvider = _gatewayProviderFactory
                 .Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations);

        gatewayPayment.SetCallback(_clock.NowWithOffset, callbackData);

        var issuer = await _receiptIssuerRepository.GetByIdAsync(receiptRequest.IssuerId);

        if (issuer is null)
        {
            throw new InvalidPaymentException();
        }

        var extractCallBackDataResult = paymentGatewayProvider.ExtractCallBackData(callbackData);

        if (!extractCallBackDataResult.Success)
        {
            gatewayPayment.FailedCallback(extractCallBackDataResult.PaymentFailedReason
                ?? PaymentGatewayFailedReason.CallbackFailed,
                extractCallBackDataResult?.Error);

            return CreateResult(receiptRequest, gatewayPayment, issuer);
        }

        // Save the receipt request and gateway payment changes
        // Preventing a duplicate request process with the concurrency on status property  
        await _unitOfWork.SaveChangesAsync();

        var callbackValidityDuration = paymentGatewayProvider.GetCallbackValidityDuration();

        var isValidCallback = receiptRequest.InternalVerifyCallbackData(gatewayPayment, extractCallBackDataResult, callbackValidityDuration);

        if (isValidCallback is false)
        {
            return CreateResult(receiptRequest, gatewayPayment, issuer);
        }

        var verifyResult = await receiptRequest.VerifyGatewayPayment(
            gatewayPayment,
            paymentGatewayProvider,
            extractCallBackDataResult);

        receiptRequest.UpdateStatusAfterVerifyGatewayPayment();

        return CreateResult(receiptRequest, gatewayPayment, issuer);
    }

    private static CallbackResult CreateResult(ReceiptRequest receiptRequest,
        ReceiptRequestGatewayPayment gatewayPayment,
        ReceiptIssuer issuer)
    {
        var issuerCallbackUrl = GetIssuerCallbackUrl(issuer, receiptRequest, gatewayPayment);

        return new CallbackResult(receiptRequest, gatewayPayment, issuerCallbackUrl);
    }

    private static string GetIssuerCallbackUrl(ReceiptIssuer issuer, ReceiptRequest receiptRequest, ReceiptRequestGatewayPayment gatewayPayment)
    {
        if (string.IsNullOrEmpty(issuer.CallbackUrl))
        {
            return string.Empty;
        }

        return issuer.CallbackUrl
                    .Replace(Constants.Parameters.ReceiptRequestIdParameter, receiptRequest.Id.ToString())
                    .Replace(Constants.Parameters.GatewayPaymentIdParameter, gatewayPayment.Id.ToString())
                    .Replace(Constants.Parameters.ReceiptRequestIssuerReferenceParameter, receiptRequest.IssuerReference)
                    .Replace(Constants.Parameters.ReceiptIssuerCodeParameter, issuer.Code.ToString())
                    .Replace(Constants.Parameters.GatewayPaymentStatusParameter, gatewayPayment.Status.ToString())
                    ;
    }
}