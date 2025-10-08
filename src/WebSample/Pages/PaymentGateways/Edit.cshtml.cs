using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Input is null && !ModelState.IsValid)
            return Page();

        var updateResult = await _commandBus.DispatchAsync<UpdatePaymentGatewayProfileCommand, Result<UpdatePaymentGatewayProfileCommandResult>>(Input!, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            Error = updateResult.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
            return Page();
        }

        return RedirectToPage("Index");
    }
}