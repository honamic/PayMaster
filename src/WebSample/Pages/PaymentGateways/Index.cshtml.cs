using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public List<GetAllPaymentGatewaysQueryResult> Profiles { get; set; } = new();
    public string? Error { get; set; }

    private readonly IQueryBus _queryBus;

    public IndexModel(IQueryBus queryBus)
    {
        _queryBus = queryBus;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var query = new GetAllPaymentGatewaysQuery()
        {
            Page = 1,
            PageSize = 100, 
        };

        var result = await _queryBus.Dispatch(query, cancellationToken);

        if (result.IsSuccessWithData)
        {
            Profiles = result.Data.Items;
        }
        else
        {
            Error = result.Messages.FirstOrDefault()?.Message ?? "خطایی رخ داده است.";
        }
    }
}