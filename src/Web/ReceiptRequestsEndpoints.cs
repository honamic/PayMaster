using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Honamic.PayMaster.Web;

public static class ReceiptRequestsEndpoints
{
    public static void MapReceiptRequestsEndpoints(IEndpointRouteBuilder app, string prefixRoute = "Payments")
    {
        var payGroup = app.MapGroup(prefixRoute)
             .WithTags("PayMaster");

        payGroup.MapGet("pay", async (HttpContext context,
          IServiceProvider services,
          [FromServices] ICommandBus commandBus,
          [AsParameters] PayReceiptRequestCommand model,
          CancellationToken cancellationToken) =>
        {
            var paycommandResult = await commandBus.DispatchAsync<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>(model, cancellationToken);

            if (paycommandResult.Status == ResultStatus.Ok)
            {
                if (paycommandResult.Data?.PayUrl == null)
                {
                    return Results.BadRequest(paycommandResult);
                }

                var url = paycommandResult.Data.PayUrl.AddParametersToUrl(paycommandResult.Data.PayParams);

                return Results.Redirect(url);
            }

            return Results.BadRequest(paycommandResult);
        });

        payGroup.MapMethods("callback/{GatewayPaymentId}/",
             ["GET", "POST"]
             , async (  string GatewayPaymentId,
                        HttpContext context,
                        [FromServices] ICommandBus commandBus,
                        CancellationToken cancellationToken) =>
        {
            string callBackData = context.ExtractCallback();

            var command = new CallBackGatewayPaymentCommand
            {
                GatewayPaymentId = GatewayPaymentId,
                CallBackData = callBackData,
            };

            var commandResult = await commandBus.DispatchAsync<CallBackGatewayPaymentCommand,
                Result<CallBackGatewayPaymentCommandResult>>(command, cancellationToken);

            return Results.Ok(commandResult);
        });
    }
}