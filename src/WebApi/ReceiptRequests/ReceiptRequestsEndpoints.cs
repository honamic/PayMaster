using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Honamic.PayMaster.WebApi.ReceiptRequests;

public static class ReceiptRequestsEndpoints
{
    public static void MapReceiptRequestsEndpoints(IEndpointRouteBuilder app, string prefixRoute)
    {
        var payGroup = app.MapGroup(prefixRoute)
             .WithTags("PayMaster");

        payGroup.MapGet("InitiatePay", async (HttpContext context,
          IServiceProvider services,
          [FromServices] ICommandBus commandBus,
          [AsParameters] InitiatePayReceiptRequestCommand model,
          CancellationToken cancellationToken) =>
        {
            var paycommandResult = await commandBus.DispatchAsync(model, cancellationToken);

            if (paycommandResult.IsSuccess)
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
             , async (string GatewayPaymentId,
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

            var commandResult = await commandBus.DispatchAsync(command, cancellationToken);

            var IssuerCallbackUrl = commandResult?.Data?.IssuerCallbackUrl;

            if (!string.IsNullOrEmpty(IssuerCallbackUrl))
                return Results.Redirect(IssuerCallbackUrl);

            return Results.Ok(commandResult);
        });
    }
}