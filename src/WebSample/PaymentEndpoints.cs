using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using System.Dynamic;
using System.Text.Json;

namespace WebSample;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(IEndpointRouteBuilder app)
    {
        var payGroup= app.MapGroup("Payment");


        payGroup.MapPost("/Payment/create/", async (HttpContext context, IServiceProvider services, decimal amount) =>
        {
            var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/Payment/callback/providerSmapleId";

            IPaymentProvider provider = PaymentFacoty.GetSampleProvider(services);

            var newPayment = PaymentStorage.Create(amount, "USD");

            var newGatewayPayment = PaymentStorage.CreateGatewayPayment(newPayment.Id);

            var createProviderResult = await provider.CreateAsync(new CreateRequest()
            {
                Amount = newPayment.Amount,
                Currency = newPayment.Currency,
                UniqueRequestId = newGatewayPayment.Id,
                CallbackUrl = callbackUrl,
            });

            PaymentStorage.SetCreateProviderRestul(newGatewayPayment.Id,
                createProviderResult.Success,
                createProviderResult.CreateReference);

            if (createProviderResult.Success)
            {
                var redirectUrl = new Uri(createProviderResult.PayUrl!);

                if (createProviderResult.PayVerb == PayVerb.Get)
                    foreach (var param in createProviderResult.PayParams)
                    {
                        redirectUrl = new Uri(redirectUrl + $"?{param.Key}={param.Value}");
                    }

                return Results.Ok(new { redirectUrl, newPayment, createProviderResult });

                //todo add param to pay url 
                //return Results.Redirect(redirectUrl.ToString());
            }

            return Results.Ok(new { newPayment, createProviderResult });
        });

        app.MapGet("/Payment/callback/{providerCode}", async (string providerCode, HttpContext context, IServiceProvider services) =>
        {
            dynamic queryObject = new ExpandoObject();
            var queryObjectDict = (IDictionary<string, object>)queryObject;

            foreach (var param in context.Request.Query)
            {
                queryObjectDict[param.Key] = param.Value.ToString();
            }

            var json = JsonSerializer.Serialize(queryObjectDict);

            IPaymentProvider provider = PaymentFacoty.GetSampleProvider(services);

            var ExtractCallBackDataResult = provider.ExtractCallBackData(json);

            var gatewayPayment = PaymentStorage.GetForVerify(
                ExtractCallBackDataResult.UniqueRequestId,
                ExtractCallBackDataResult.CreateReference,
                ExtractCallBackDataResult.Success,
                ExtractCallBackDataResult.Error);

            if (!ExtractCallBackDataResult.Success)
            {
                return Results.Ok(new { ExtractCallBackDataResult, gatewayPayment });
            }

            VerifyRequest verifyRequest = new()
            {
                PatmentInfo = new VerifyRequestPatmentInfo
                {
                    Amount = gatewayPayment!.Amount,
                    UniqueRequestId = gatewayPayment.Id,
                    CreateReference = gatewayPayment.CreateReference,
                },
                CallBackData = ExtractCallBackDataResult.CallBack
            };

            var verifyResult = await provider.VerifyAsync(verifyRequest);

            PaymentStorage.SaveVerify(verifyResult, gatewayPayment);

            if (verifyResult.Success)
            {

            }


            return Results.Ok(new { verifyResult, ExtractCallBackDataResult, gatewayPayment });
        });

    }
}
