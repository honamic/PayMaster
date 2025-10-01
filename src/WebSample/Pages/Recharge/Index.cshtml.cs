using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.WebApi.Helpers;
using Honamic.PayMaster.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSample.Pages.Recharge;

public class RechargeModel : PageModel
{
    private readonly IPayMasterFacade PayMasterFacade;

    public RechargeModel(IPayMasterFacade payMasterFacade)
    {
        PayMasterFacade = payMasterFacade;
    }

    [BindProperty]
    public CreateReceiptRequestCommand Input { get; set; } = new();
    public string Error { get; set; } = string.Empty;

    public List<GetActivePaymentGatewaysQueryResult> PaymentGateways { get; set; } = new();

    [BindProperty]
    public string? SelectedGateway { get; set; }


    public Task OnGet(CancellationToken cancellationToken)
    {
        return LoadGateways(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return await ShowPage(cancellationToken);
        }

        if (!ModelState.IsValid)
            return await ShowPage(cancellationToken);

        var createResult = await PayMasterFacade.CreateReceiptRequest(Input, cancellationToken);

        if (!createResult.IsSuccess)
        {
            Error = createResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            return await ShowPage(cancellationToken);
        }

        var paycommand = new PayReceiptRequestCommand
        {
            ReceiptRequestId = createResult.Data!.Id,
        };

        var paycommandResult = await PayMasterFacade.PayReceiptRequest(paycommand, cancellationToken);

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
        var gatewaysResult = await PayMasterFacade.GetActivePaymentGateways(cancellationToken);
        
        PaymentGateways.Add(new GetActivePaymentGatewaysQueryResult
        {
            Id = 0,
            Title = "درگاه پیش فرض سیستم",
            Code = "Default",
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
