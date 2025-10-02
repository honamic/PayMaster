using Honamic.PayMaster.PaymentProvider.Sandbox.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web.Pages;

public class SandboxPayModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public SanboxRequestDataModel Request { get; set; } = default!;

    public IActionResult OnGet()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Page();
    }

    public IActionResult OnPostSuccess()
    {
        var callbackData = new SanboxCallBackDataModel
        {
            Status = "OK",
            Amount = Request.Amount,
            Currency = Request.Currency,
            Pan = "401234******9999",
            Token = Request.Token,
            PayId = Request.PayId,
            PayRequestId = Request.UniqueRequestId,
            TrackingNumber = DateTime.Now.ToString("yyyyMMddHHmmss")
        };

        var url = AddParametersToUrl(Request.CallbackUrl, callbackData);
        return Redirect(url);
    }

    public IActionResult OnPostCancel()
    {
        var callbackData = new SanboxCallBackDataModel
        {
            Status = "NOK",
            Amount = Request.Amount,
            Currency = Request.Currency,
            Token = Request.Token,
            PayId = Request.PayId,
            PayRequestId = Request.UniqueRequestId,
        };

        var url = AddParametersToUrl(Request.CallbackUrl, callbackData);
        return Redirect(url);
    }


    public static string AddParametersToUrl(string baseUrl, SanboxCallBackDataModel dataModel)
    {
        var uriBuilder = new UriBuilder(baseUrl);

        var parsed = QueryHelpers.ParseQuery(uriBuilder.Query);

        var qb = new QueryBuilder(
            parsed.SelectMany(kvp => kvp.Value, (kvp, val) =>
                new KeyValuePair<string, string>(kvp.Key, val ?? string.Empty))
        );

        if (dataModel is null)
            throw new Exception("dataModel is null");

        qb.Add(nameof(dataModel.Status), dataModel.Status!);
        qb.Add(nameof(dataModel.Amount), dataModel.Amount.ToString(CultureInfo.InvariantCulture));
        qb.Add(nameof(dataModel.Currency), dataModel.Currency);
        qb.Add(nameof(dataModel.Token), dataModel.Token);
        qb.Add(nameof(dataModel.PayId), dataModel.PayId);
        qb.Add(nameof(dataModel.PayRequestId), dataModel.PayRequestId);

        qb.Add(nameof(dataModel.TrackingNumber), dataModel.TrackingNumber ?? string.Empty);
        qb.Add(nameof(dataModel.Pan), dataModel.Pan ?? string.Empty);

        uriBuilder.Query = qb.ToQueryString().Value.TrimStart('?');

        return uriBuilder.Uri.ToString();

    }

    public string MarchanetName()
    {
        var name = Request.MerchantName ?? "نام پذیرنده";

        if(!string.IsNullOrEmpty(Request.GatewayNote))
        {
            name = $"{name}({Request.GatewayNote})";
        }

        return name;
    }

}