using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace WebSample;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(IEndpointRouteBuilder app)
    {
        var payGroup = app.MapGroup("sample");

        payGroup.MapGet("sample/receipt/createAndPay/", async (HttpContext context,
          IServiceProvider services,
          [FromServices] ICommandBus commandBus,
          [AsParameters] CreateReceiptRequestCommand model,
          CancellationToken cancellationToken) =>
        {
            var createResult = await commandBus.DispatchAsync<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>(model, cancellationToken);

            if (createResult.Status != ResultStatus.Ok)
            {
                return Results.BadRequest(createResult);
            }

            var paycommand = new PayReceiptRequestCommand
            {
                ReceiptRequestId = createResult.Data!.Id,
            };

            var paycommandResult = await commandBus.DispatchAsync<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>(paycommand, cancellationToken);

            if (paycommandResult.Status == ResultStatus.Ok
)
            {
                if (paycommandResult.Data?.PayUrl == null)
                {
                    return Results.BadRequest(paycommandResult);
                }

                var url = paycommandResult.Data.PayUrl.AddParametersToUrl(paycommandResult.Data.PayParams);

                return Results.Redirect(url, false, true);

            }

            return Results.BadRequest(paycommandResult);
        });

        payGroup.MapPost("sample/receipt/create/", async (HttpContext context,
                IServiceProvider services,
               [FromServices] ICommandBus commandBus,
                [AsParameters] CreateReceiptRequestCommand model,
                  CancellationToken cancellationToken) =>
        {
            var commandResult = await commandBus.DispatchAsync<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>(model, cancellationToken);

            return Results.Ok(commandResult);
        });

    }
}