using Honamic.PayMaster.PaymentProvider.Sandbox.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;

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
        var query = QueryHelpers.ParseQuery(uriBuilder.Query);

        query[nameof(dataModel.Status)] = dataModel.Status;
        query[nameof(dataModel.Amount)] = dataModel.Amount.ToString();
        query[nameof(dataModel.Currency)] = dataModel.Currency;
        query[nameof(dataModel.Token)] = dataModel.Token;
        query[nameof(dataModel.PayId)] = dataModel.PayId;
        query[nameof(dataModel.PayRequestId)] = dataModel.PayRequestId;
        query[nameof(dataModel.TrackingNumber)] = dataModel.TrackingNumber;
        query[nameof(dataModel.Pan)] = dataModel.Pan;

        uriBuilder.Query = query.ToString();

        return uriBuilder.ToString();
    }
}