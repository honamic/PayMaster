using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Queries; 
using Honamic.PayMaster.WebApi.Helpers;
using Honamic.PayMaster.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSample.Pages.Recharge;

public class RepayModel : PageModel
{
    private readonly IPayMasterFacade _payMasterFacade;

    public RepayModel(IPayMasterFacade payMasterFacade)
    {
        _payMasterFacade = payMasterFacade;
    }

    [BindProperty]
    public RepayReceiptRequestCommand Input { get; set; } = new();


    public GetPublicReceiptRequestQueryResult? Receipt { get; set; }


    public string? Error { get; set; }

    public List<GetActivePaymentGatewaysQueryResult> PaymentGateways { get; set; } = new();

    [BindProperty]
    public string? SelectedGateway { get; set; }


    public async Task OnGet([FromRoute] long ReceiptRequestId, CancellationToken cancellationToken)
    {
        Input.ReceiptRequestId = ReceiptRequestId;

        await LoadReceiptRequest(ReceiptRequestId, cancellationToken);

        await LoadGateways(cancellationToken);
    }

    private async Task LoadReceiptRequest(long ReceiptRequestId, CancellationToken cancellationToken)
    {
        var result = await _payMasterFacade.GetPublicReceiptRequest(new GetPublicReceiptRequestQuery
        {
            Id = ReceiptRequestId
        }, cancellationToken);


        Receipt = result.Data;
        Error = result.Messages.FirstOrDefault()?.Message;

        if (Receipt is null && Error is null)
        {
            Error = "اطلاعات درخواستی یافت نشد.";
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await ShowPage(cancellationToken);
        }

        if (!ModelState.IsValid)
            return await ShowPage(cancellationToken);

        var createResult = await _payMasterFacade.RepayReceiptRequest(Input, cancellationToken);

        if (!createResult.IsSuccess)
        {
            Error = createResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            return await ShowPage(cancellationToken);
        }

        var paycommand = new InitiatePayReceiptRequestCommand
        {
            ReceiptRequestId = createResult.Data!.Id,
        };

        var paycommandResult = await _payMasterFacade.InitiatePayReceiptRequest(paycommand, cancellationToken);

        if (paycommandResult.IsSuccessWithData)
        {
            var url = paycommandResult.Data.PayUrl?.AddParametersToUrl(paycommandResult.Data.PayParams);

            return Redirect(url);

        }

        Error = paycommandResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
        return await ShowPage(cancellationToken);

    }

    private async Task<IActionResult> ShowPage(CancellationToken cancellationToken)
    {
        await LoadGateways(cancellationToken);

        return Page();
    }

    private async Task LoadGateways(CancellationToken cancellationToken)
    {
        var gatewaysResult = await _payMasterFacade.GetActivePaymentGateways(cancellationToken);

        PaymentGateways.Add(new GetActivePaymentGatewaysQueryResult
        {
            Id = -1,
            Title = "درگاه پیش فرض سیستم",
            Code = "",
        });

        if (gatewaysResult.IsSuccessWithData)
        {
            PaymentGateways.AddRange(gatewaysResult.Data);
        }
        else
        {
            Error = gatewaysResult.Messages.FirstOrDefault()?.Message ?? "خط در بارگذاری درگاهها";
        }
    }

}
