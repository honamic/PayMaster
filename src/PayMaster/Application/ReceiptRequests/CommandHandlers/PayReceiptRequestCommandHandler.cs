using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class PayReceiptRequestCommandHandler : ICommandHandler<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>
{
    private readonly IReceiptRequestRepository _receiptRequestRepository;
    private readonly ICreatePaymentDomainService _createPaymentDomainService;

    public PayReceiptRequestCommandHandler(
        IReceiptRequestRepository receiptRequestRepository,
        ICreatePaymentDomainService createPaymentDomainService)
    {
        _receiptRequestRepository = receiptRequestRepository;
        _createPaymentDomainService = createPaymentDomainService;
    }

    public async Task<Result<PayReceiptRequestCommandResult>> HandleAsync(PayReceiptRequestCommand command, CancellationToken cancellationToken)
    {
        var receiptRequest = await _receiptRequestRepository
                        .GetAsync(c => c.Id == command.GetReceiptRequestIdAsLong());

        if (receiptRequest is null)
        {
            throw new ArgumentException("فیش وجود ندارد.");
        }

        var createResult =await _createPaymentDomainService.CreatePaymentAsync(receiptRequest);

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

        var result = new Result<PayReceiptRequestCommandResult>();

        result.AppendError("خطا در آماده سازی برای ارسال به درگاه.");

        return  result;
    }
}