using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Web.Helpers;
using Honamic.PayMaster.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace WebSample;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var payGroup = app.MapGroup("sample");

        payGroup.MapGet("sample/receipt/createAndPay/", async (
            [FromServices] IPayMasterFacade payMasterFacade,
            [AsParameters] CreateReceiptRequestCommand model,
            CancellationToken cancellationToken) =>
        {
            var createResult = await payMasterFacade.CreateReceiptRequest(model, cancellationToken);

            if (!createResult.IsSuccess)
            {
                return Results.BadRequest(createResult);
            }

            var paycommand = new PayReceiptRequestCommand
            {
                ReceiptRequestId = createResult.Data!.Id,
            };

            var paycommandResult = await payMasterFacade.PayReceiptRequest(paycommand, cancellationToken);

            if (paycommandResult.IsSuccess)
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

        payGroup.MapPost("sample/receipt/create/", async ( [FromServices] IPayMasterFacade payMasterFacade,
                [AsParameters] CreateReceiptRequestCommand model,
                CancellationToken cancellationToken) =>
        {
            var commandResult = await payMasterFacade.CreateReceiptRequest(model, cancellationToken);

            return Results.Ok(commandResult);
        });

    }
}