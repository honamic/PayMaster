using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading;

public class EditModel : PageModel
{
    public string? Error { get; set; }

    private readonly IQueryBus _queryBus;
    private readonly ICommandBus _commandBus;

    public EditModel(IQueryBus queryBus, ICommandBus commandBus)
    {
        _queryBus = queryBus;
        _commandBus = commandBus;
    }

    [BindProperty]
    public UpdatePaymentGatewayProfileCommand? Input { get; set; }
    public List<SelectListItem> PaymentGatewayProviders { get; set; }

    public async Task<IActionResult> OnGetAsync(long id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentGatewayQuery()
        {
            Id = id
        };

        var result = await _queryBus.Dispatch(query, cancellationToken);

        if (result.IsSuccessWithData)
        {
            var profile = result.Data;

            Input = new UpdatePaymentGatewayProfileCommand
            {
                Id = profile.Id,
                Title = profile.Title,
                Code = profile.Code,
                ProviderType = profile.ProviderType,
                LogoPath = profile.LogoPath,
                JsonConfigurations = profile.JsonConfigurations,
                Enabled = profile.Enabled,
                MinimumAmount = profile.MinimumAmount,
                MaximumAmount = profile.MaximumAmount,
            };
        }
        else if (result.IsSuccess)
        {
            return NotFound();
        }
        else
        {
            Error = result.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
        }

        await LoadPaymentGatewayProviders(cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Input is null && !ModelState.IsValid) {
            await LoadPaymentGatewayProviders(cancellationToken);
            return Page();
        }

        var updateResult = await _commandBus.DispatchAsync<UpdatePaymentGatewayProfileCommand, Result<UpdatePaymentGatewayProfileCommandResult>>(Input!, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            Error = updateResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            await LoadPaymentGatewayProviders(cancellationToken);
            return Page();
        }

        return RedirectToPage("Index");
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