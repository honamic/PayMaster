using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using IdGen;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class CallBackGatewayPaymentCommandHandler : ICommandHandler<CallBackGatewayPaymentCommand, Result<CallBackGatewayPaymentCommandResult>>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly ICreatePaymentDomainService _createPaymentDomainService;
    private readonly IPaymentGatewayProviderRepository _repository;
    private readonly IPaymentGatewayProviderFactory _factory;
    private readonly IClock _clock;

    public CallBackGatewayPaymentCommandHandler(
        IReceiptRequestRepository receiptRequestRepository,
        ICreatePaymentDomainService createPaymentDomainService,
        IPaymentGatewayProviderFactory factory,
        IPaymentGatewayProviderRepository repository,
        IClock clock)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _createPaymentDomainService = createPaymentDomainService;
        _factory = factory;
        _repository = repository;
        _clock = clock;
    }

    public async Task<Result<CallBackGatewayPaymentCommandResult>> HandleAsync(CallBackGatewayPaymentCommand command, CancellationToken cancellationToken)
    {
        ReceiptRequest? receiptRequest = null;
        ReceiptRequestGatewayPayment? gatewayPayment = null;
        IPaymentGatewayProvider? paymentGatewayProvider = null;
        ExtractCallBackDataResult ExtractCallBackDataResult;
        var gatewayPaymentId = command.GetGatewayPaymentIdAsLong();
        var gatewayProviderId = command.GetGatewayProviderIdAsLong();

        if (gatewayPaymentId.HasValue)
        {
            receiptRequest = await _receiptRequestRepository
                  .GetByGatewayPaymentIDAsync(gatewayPaymentId.Value);

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

            var gatewayProvider = await _repository.GetAsync(c => c.Id == gatewayProviderId);

            if (gatewayProvider == null)
            {
                throw new InvalidOperationException("درگاه پرداخت شناسایی نشد.");
            }

            paymentGatewayProvider = _factory.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

            if (paymentGatewayProvider == null)
            {
                throw new InvalidOperationException("درگاه پرداخت ساخته نشد.");
            }

            ExtractCallBackDataResult = paymentGatewayProvider.ExtractCallBackData(command.CallBackData);
        }
        else
        {
            var gatewayProvider = await _repository.GetAsync(c => c.Id == gatewayProviderId);

            if (gatewayProvider == null)
            {
                throw new InvalidOperationException("درگاه پرداخت شناسایی نشد.");
            }

            paymentGatewayProvider = _factory.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations);

            if (paymentGatewayProvider == null)
            {
                throw new InvalidOperationException("درگاه پرداخت ساخته نشد.");
            }

            ExtractCallBackDataResult = paymentGatewayProvider.ExtractCallBackData(command.CallBackData);

            if (ExtractCallBackDataResult.UniqueRequestId.HasValue)
            {
                gatewayPaymentId = ExtractCallBackDataResult.UniqueRequestId;
                receiptRequest = await _receiptRequestRepository.GetByGatewayPaymentIDAsync(gatewayPaymentId.Value);
            }
            else if (!string.IsNullOrEmpty(ExtractCallBackDataResult.CreateReference))
            {
                receiptRequest = await _receiptRequestRepository
                    .GetByGatewayPaymentCreateReferenceAsync
                    (ExtractCallBackDataResult.CreateReference, gatewayProviderId);

                gatewayPaymentId = receiptRequest?.GatewayPayments
                    .First(c => c.CreateReference == ExtractCallBackDataResult.CreateReference)
                    .Id;
            }
            else
            {
                throw new ArgumentException("پرداخت معتبر نیست.");
            }


            if (receiptRequest is null)
            {
                throw new ArgumentException("پرداخت معتبر نیست.");
            }

            gatewayPayment = receiptRequest.GetGatewayPayment(gatewayPaymentId!.Value);

            if (gatewayPayment is null)
            {
                throw new ArgumentException("پرداخت معتبر نیست.");
            }
        }

        var result = await receiptRequest.StartCallBackForGatewayPayment(gatewayPayment, paymentGatewayProvider, _clock, ExtractCallBackDataResult);

        return new CallBackGatewayPaymentCommandResult
        {
            ReceiptRequestId = receiptRequest.Id.ToString(CultureInfo.InvariantCulture),
            Status = receiptRequest.Status,
            IssuerReference = receiptRequest.IssuerReference,
            PartyReference = receiptRequest.PartyReference,
            Amount = receiptRequest.Amount,
            Currency = receiptRequest.Currency,
            AdditionalData = receiptRequest.AdditionalData,
            GatewayPayments = new List<CallBackGatewayPaymentGatewayPaymentsCommandResult>
            {
                new CallBackGatewayPaymentGatewayPaymentsCommandResult
                {
                   Id= gatewayPayment.Id.ToString(CultureInfo.InvariantCulture),
                   Amount= gatewayPayment.Amount,
                   Currency= gatewayPayment.Currency,
                   Status= gatewayPayment.Status,
                   StatusDescription= gatewayPayment.StatusDescription,
                   FailedReason= gatewayPayment.FailedReason,
                }
            },
            Issuer = new
            {
                //todo added issuer fields:
                // CallbackUrl
                // ShowPaymentResultPage
            }
        };
    }
}