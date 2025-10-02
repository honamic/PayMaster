using Honamic.PayMaster.PaymentProvider.Sandbox.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web;

public static class SandboxEndpoints
{
    public static void MapSandboxPayEndpoints(IEndpointRouteBuilder app, string sandboxPath = "/paymaster/sandbox/pay")
    {
        app.MapGet(sandboxPath,
            async ([AsParameters] SanboxRequestDataModel request, CancellationToken cancellationToken) =>
             {

                 var successCallbackData = new SanboxCallBackDataModel
                 {
                     Status = "OK",
                     Amount = request.Amount,
                     Currency = request.Currency,
                     Pan = "401234******9999",
                     Token = request.Token,
                     PayId = request.PayId,
                     PayRequestId = request.UniqueRequestId,
                     TrackingNumber = DateTime.Now.ToString("yyyyMMddHHmmss")
                 };

                 var successUrl = AddParametersToUrl( request.CallbackUrl, successCallbackData);

                 var cancelCallbackData = new SanboxCallBackDataModel
                 {
                     Status = "NOK",
                     Amount = request.Amount,
                     Currency = request.Currency, 
                     Token = request.Token,
                     PayId = request.PayId,
                     PayRequestId = request.UniqueRequestId,
                  };

                 var cancelUrl = AddParametersToUrl(request.CallbackUrl, cancelCallbackData);

                 return Results.Ok(new { success = successUrl, cancel = cancelUrl });
             }).WithTags("PayMaster");
    }


    public static string AddParametersToUrl(this string baseUrl, SanboxCallBackDataModel dataModel)
    {
        var uriBuilder = new UriBuilder(baseUrl);

        var parsed = QueryHelpers.ParseQuery(uriBuilder.Query); 

        var qb = new QueryBuilder(
            parsed.SelectMany(kvp => kvp.Value, (kvp, val) =>
                new KeyValuePair<string, string>(kvp.Key, val ?? string.Empty))
        );

        if(dataModel is null )
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
}