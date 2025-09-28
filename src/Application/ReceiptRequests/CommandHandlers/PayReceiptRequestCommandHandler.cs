using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class PayReceiptRequestCommandHandler : ICommandHandler<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly IPaymentGatewayInitializationService _paymentGatewayInitializationService;
    private readonly IOptions<PayMasterOptions> _payMasterOptions;

    public PayReceiptRequestCommandHandler(
        IReceiptRequestRepository receiptRequestRepository,
        IPaymentGatewayInitializationService paymentGatewayInitializationService,
        IOptions<PayMasterOptions> payMasterOptions)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _paymentGatewayInitializationService = paymentGatewayInitializationService;
        _payMasterOptions = payMasterOptions;
    }

    public async Task<Result<PayReceiptRequestCommandResult>> HandleAsync(PayReceiptRequestCommand command, CancellationToken cancellationToken)
    {
        var result = new Result<PayReceiptRequestCommandResult>();

        var receiptRequest = await _receiptRequestRepository.GetByIdAsync(command.GetReceiptRequestIdAsLong());

        if (receiptRequest is null)
        {
            result.SetStatusAsNotFound($"قبض با شناسه {command.ReceiptRequestId} وجود ندارد.");
            return result;
        }

        var createResult = await _paymentGatewayInitializationService.InitializePaymentAsync(receiptRequest, _payMasterOptions.Value.CallBackUrl);

        if (createResult.Success)
        {
            result.Data = new PayReceiptRequestCommandResult
            {
                ReceiptRequestId = receiptRequest.Id.ToString(CultureInfo.InvariantCulture),
                PayParams = createResult.PayParams,
                PayUrl = createResult.PayUrl,
                PayVerb = createResult.PayVerb,
            };

            result.SetSuccess();
        }
        else
        {
            result.SetStatusAsUnhandledException("خطا در آماده سازی برای ارسال به درگاه.");
        }

        return result;
    }
}