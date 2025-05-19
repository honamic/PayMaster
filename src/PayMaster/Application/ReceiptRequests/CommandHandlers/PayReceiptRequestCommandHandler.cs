using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Core.ReceiptRequests;
using Honamic.PayMaster.Core.ReceiptRequests.Services;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class PayReceiptRequestCommandHandler : ICommandHandler<PayReceiptRequestCommand, PayReceiptRequestCommandResult>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly ICreatePaymentDomianService _createPaymentDomianService;

    public PayReceiptRequestCommandHandler(
        IReceiptRequestRepository receiptRequestRepository,
        ICreatePaymentDomianService paymentProcessingService)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _createPaymentDomianService = paymentProcessingService;
    }

    public async Task<PayReceiptRequestCommandResult> HandleAsync(PayReceiptRequestCommand command, CancellationToken cancellationToken)
    {
        // todo move to config
        var callbackUrl = "https://localhost:7121/PayMaster/Callback/";

        var receiptRequest = await _receiptRequestRepository
                        .GetAsync(c => c.Id == command.ReceiptRequestIdAslong());

        if (receiptRequest is null)
        {
            throw new ArgumentException("فیش وجود ندارد.");
        }

        var createResult =await _createPaymentDomianService.CreatePaymentAsync(receiptRequest, callbackUrl);

        if (createResult.Success)
        {
            return new PayReceiptRequestCommandResult
            {
                ReceiptRequestId = receiptRequest.Id.ToString(CultureInfo.InvariantCulture),
                PayParams = createResult.PayParams,
                PayUrl = createResult.PayUrl,
                PayVerb = createResult.PayVerb,
            };
        }

        return new PayReceiptRequestCommandResult
        {
            ReceiptRequestId = receiptRequest.Id.ToString(CultureInfo.InvariantCulture)
        };
    }
}