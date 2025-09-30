using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Honamic.PayMaster.WebApi.Helpers;

namespace WebSample.Pages.Recharge;

public class RechargeModel : PageModel
{
    public RechargeModel(IPayMasterFacade payMasterFacade)
    {
        PayMasterFacade = payMasterFacade;
    }

    [BindProperty]
    public CreateReceiptRequestCommand Input { get; set; } = new();
    public string Error { get; set; } = string.Empty;

    public List<SelectListItem> PaymentGateways { get; set; } = new();

    [BindProperty]
    public string? SelectedGateway { get; set; }

    [Inject]
    IPayMasterFacade PayMasterFacade { get; set; }

    public void OnGet()
    {
        LoadGateways();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        LoadGateways();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!ModelState.IsValid)
            return Page();

        var createResult = await PayMasterFacade.CreateReceiptRequest(Input, cancellationToken);

        if (!createResult.IsSuccess)
        {
            Error = createResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            return Page();
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
        return Page();

    }


    private void LoadGateways()
    {
        PaymentGateways = new List<SelectListItem>
    {
        new("پیش فرض", "default"),
        new("درگاه تست", "sandbox"),
        new("زرین‌پال", "zarinpal"),
        new("دیجی‌پی", "digipay"),
        new("سداد", "sadad")
    };
    }

}
