using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text.Json;

namespace WebSample;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("sample/receipt/createAndPay/", async (HttpContext context,
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

            if (paycommandResult.Status==ResultStatus.Ok 
                && paycommandResult.Data?.PayUrl != null)
            {
                return Results.Redirect(paycommandResult.Data.PayUrl);
            }

            return Results.BadRequest(paycommandResult);
        });

        var payGroup = app.MapGroup("PaymentMaster");


        payGroup.MapPost("/receipt/create/", async (HttpContext context,
                IServiceProvider services,
               [FromServices] ICommandBus commandBus,
                [AsParameters] CreateReceiptRequestCommand model,
                  CancellationToken cancellationToken) =>
        {
            var commandResult = await commandBus.DispatchAsync<CreateReceiptRequestCommand, Result<CreateReceiptRequestCommandResult>>(model, cancellationToken);

            return Results.Ok(commandResult);
        });

        payGroup.MapPost("/receipt/pay/", async (HttpContext context,
          IServiceProvider services,
         [FromServices] ICommandBus commandBus,
          [AsParameters] PayReceiptRequestCommand model,
            CancellationToken cancellationToken) =>
        {
            var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/Payment/callback/providerSmapleId";

            var commandResult = await commandBus.DispatchAsync<PayReceiptRequestCommand, Result<PayReceiptRequestCommandResult>>(model, cancellationToken);

            return Results.Ok(commandResult);
        });

        payGroup.MapPost("/callback/{GatewayProviderId}/{GatewayPaymentId?}/", async (
            string GatewayProviderId,
            string? GatewayPaymentId,
            HttpContext context,
            [FromServices] ICommandBus commandBus,
            CancellationToken cancellationToken) =>
        {
            string callBackData = ExtractCallBack(context);

            var commandResult = await SendCallbackCommand(GatewayProviderId, GatewayPaymentId, commandBus, callBackData, cancellationToken);

            return Results.Ok(commandResult);
        });

        payGroup.MapGet("/callback/{GatewayProviderId}/{GatewayPaymentId?}/", async (
                string GatewayProviderId,
                string? GatewayPaymentId,
                HttpContext context,
                [FromServices] ICommandBus commandBus,
                CancellationToken cancellationToken) =>
        {
            string callBackData = ExtractCallBack(context);

            var commandResult = await SendCallbackCommand(GatewayProviderId, GatewayPaymentId, commandBus, callBackData, cancellationToken);

            return Results.Ok(commandResult);
        });

    }

    private static async Task<Result<CallBackGatewayPaymentCommandResult>> SendCallbackCommand(string GatewayProviderId, string? GatewayPaymentId, ICommandBus commandBus, string callBackData, CancellationToken cancellationToken)
    {
        var command = new CallBackGatewayPaymentCommand
        {
            GatewayProviderId = GatewayProviderId,
            GatewayPaymentId = GatewayPaymentId,
            CallBackData = callBackData,
        };

        var commandResult = await commandBus.DispatchAsync<CallBackGatewayPaymentCommand,
            Result<CallBackGatewayPaymentCommandResult>>
        (command, cancellationToken);
        return commandResult;
    }

    private static string ExtractCallBack(HttpContext context)
    {
        dynamic queryObject = new ExpandoObject();
        var queryObjectDict = (IDictionary<string, object>)queryObject;


        if (HttpMethods.IsPost(context.Request.Method))
        {
            foreach (var param in context.Request.Form)
            {
                queryObjectDict[param.Key] = param.Value.ToString();
            }
        }
        else
        {
            foreach (var param in context.Request.Query)
            {
                queryObjectDict[param.Key] = param.Value.ToString();
            }
        }

        var callBackData = JsonSerializer.Serialize(queryObjectDict);
        return callBackData;
    }
}