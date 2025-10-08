using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    [BindProperty]
    public CreatePaymentGatewayProfileCommand Input { get; set; } = new()
    {
        Code = string.Empty,
        JsonConfigurations = string.Empty,
        Title = string.Empty,
        ProviderType = string.Empty,
    };

    public List<SelectListItem> PaymentGatewayProviders { get; set; }
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;
    public string? ErrorMessage { get; set; }

    public CreateModel(ICommandBus commandBus, IQueryBus queryBus)
    {
        _commandBus = commandBus;
        _queryBus = queryBus;
    }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        await LoadPaymentGatewayProviders(cancellationToken);

    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadPaymentGatewayProviders(cancellationToken);
            return Page();
        }

        var updateResult = await _commandBus.DispatchAsync<CreatePaymentGatewayProfileCommand, Result<CreatePaymentGatewayProfileCommandResult>>(Input!, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            ErrorMessage = updateResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";

            await LoadPaymentGatewayProviders(cancellationToken);
            return Page();
        }

        return RedirectToPage("/PaymentGateways/Index");
    }


    private async Task LoadPaymentGatewayProviders(CancellationToken cancellationToken)
    {
        var query = new GetAllLoaddedPaymentGatewayProvidersQuery();

        var providersResult = await _queryBus.Dispatch(query, cancellationToken);

        if (providersResult.IsSuccessWithData)
        {
            PaymentGatewayProviders = providersResult.Data
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProviderType,
                        Text = p.DisplayName
                    })
                    .ToList();
        }
    }
}