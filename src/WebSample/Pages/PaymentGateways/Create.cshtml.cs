using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateModel : PageModel
{
    [BindProperty]
    public CreatePaymentGatewayProfileCommand Input { get; set; } = new ()
    {
        Code = string.Empty,
        JsonConfigurations=string.Empty,
        Title=string.Empty,
        ProviderType=string.Empty,
    };

    private readonly ICommandBus _commandBus;
    public string? ErrorMessage { get; set; }

    public CreateModel( ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public void OnGet(CancellationToken cancellationToken)
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updateResult = await _commandBus.DispatchAsync<CreatePaymentGatewayProfileCommand, Result<CreatePaymentGatewayProfileCommandResult>>(Input!, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            ErrorMessage = updateResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            return Page();
        } 

        return RedirectToPage("/PaymentGateways/Index");
    }
}