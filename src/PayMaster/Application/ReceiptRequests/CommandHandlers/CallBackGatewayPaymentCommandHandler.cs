using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using System.Globalization;

namespace Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
internal class CallBackGatewayPaymentCommandHandler : ICommandHandler<CallBackGatewayPaymentCommand, Result<CallBackGatewayPaymentCommandResult>>
{
    private readonly ICallbackGatewayPaymentDomainService _callBackGatewayPaymentDomainService;

    public CallBackGatewayPaymentCommandHandler(
        ICallbackGatewayPaymentDomainService callBackGatewayPaymentDomainService)
    {
        _callBackGatewayPaymentDomainService = callBackGatewayPaymentDomainService;
    }

    public async Task<Result<CallBackGatewayPaymentCommandResult>> HandleAsync(
        CallBackGatewayPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var gatewayPaymentId = command.GetGatewayPaymentIdAsLong();
        var receiptRequestId = command.GetReceiptRequestIdAsLong();

        var result = await _callBackGatewayPaymentDomainService.ProcessCallbackAsync(
            receiptRequestId, gatewayPaymentId, command.CallBackData);

        return new CallBackGatewayPaymentCommandResult
        {
            ReceiptRequestId = result.ReceiptRequest.Id.ToString(CultureInfo.InvariantCulture),
            Status = result.ReceiptRequest.Status,
            IssuerReference = result.ReceiptRequest.IssuerReference,
            IssuerCallbackUrl = result.IssuerCallbackUrl,
            PartyReference = result.ReceiptRequest.PartyReference,
            Amount = result.ReceiptRequest.Amount,
            Currency = result.ReceiptRequest.Currency,
            AdditionalData = result.ReceiptRequest.AdditionalData,
            GatewayPayments = new List<CallBackGatewayPaymentGatewayPaymentsCommandResult>
                    {
                        new CallBackGatewayPaymentGatewayPaymentsCommandResult
                        {
                            Id = result.GatewayPayment.Id.ToString(CultureInfo.InvariantCulture),
                            Amount = result.GatewayPayment.Amount,
                            Currency = result.GatewayPayment.Currency,
                            Status = result.GatewayPayment.Status,
                            StatusDescription = result.GatewayPayment.StatusDescription,
                            FailedReason = result.GatewayPayment.FailedReason,
                        }
                    },
        };
    }
}