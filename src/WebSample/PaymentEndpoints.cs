using Honamic.Framework.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Microsoft.AspNetCore.Mvc;

namespace WebSample;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(IEndpointRouteBuilder app)
    {
        var payGroup = app.MapGroup("PaymentMaster");


        payGroup.MapPost("/receipt/create/", async (HttpContext context,
                IServiceProvider services,
               [FromServices] ICommandBus commandBus,
                [AsParameters] CreateReceiptRequestCommand model,
                  CancellationToken cancellationToken) =>
        {
            var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/Payment/callback/providerSmapleId";

            var commandResult =await commandBus.DispatchAsync<CreateReceiptRequestCommand, CreateReceiptRequestCommandResult>(model, cancellationToken);

            return Results.Ok(commandResult);
        });

        payGroup.MapPost("/receipt/pay/", async (HttpContext context,
          IServiceProvider services,
         [FromServices] ICommandBus commandBus,
          [AsParameters] PayReceiptRequestCommand model,
            CancellationToken cancellationToken) =>
        {
            var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/Payment/callback/providerSmapleId";

            var commandResult = await commandBus.DispatchAsync<PayReceiptRequestCommand, PayReceiptRequestCommandResult>(model, cancellationToken);

            return Results.Ok(commandResult);
        });

        //app.MapGet("/Payment/callback/{providerCode}", async (string providerCode, HttpContext context, IServiceProvider services) =>
        //{
        //    dynamic queryObject = new ExpandoObject();
        //    var queryObjectDict = (IDictionary<string, object>)queryObject;

        //    foreach (var param in context.Request.Query)
        //    {
        //        queryObjectDict[param.Key] = param.Value.ToString();
        //    }

        //    var json = JsonSerializer.Serialize(queryObjectDict);

        //    IPaymentGatewayProvider provider = PaymentFacoty.GetSampleProvider(services);

        //    var ExtractCallBackDataResult = provider.ExtractCallBackData(json);

        //    var gatewayPayment = PaymentStorage.GetForVerify(
        //        ExtractCallBackDataResult.UniqueRequestId,
        //        ExtractCallBackDataResult.CreateReference,
        //        ExtractCallBackDataResult.Success,
        //        ExtractCallBackDataResult.Error);

        //    if (!ExtractCallBackDataResult.Success)
        //    {
        //        return Results.Ok(new { ExtractCallBackDataResult, gatewayPayment });
        //    }

        //    VerifyRequest verifyRequest = new()
        //    {
        //        PatmentInfo = new VerifyRequestPatmentInfo
        //        {
        //            Amount = gatewayPayment!.Amount,
        //            UniqueRequestId = gatewayPayment.Id,
        //            CreateReference = gatewayPayment.CreateReference,
        //        },
        //        CallBackData = ExtractCallBackDataResult.CallBack
        //    };

        //    var verifyResult = await provider.VerifyAsync(verifyRequest);

        //    PaymentStorage.SaveVerify(verifyResult, gatewayPayment);

        //    if (verifyResult.Success)
        //    {

        //    }


        //    return Results.Ok(new { verifyResult, ExtractCallBackDataResult, gatewayPayment });
        //});

    }
}


//public static class PaymentFacoty
//{
//    private static ZarinPalConfigurations zarinPalConfig = new()
//    {
//        ApiAddress = "https://sandbox.zarinpal.com/",
//        PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
//        MerchantId = "3614255c-8e1a-4729-90d8-92f4119a6489",
//    };

//    private static PayPalConfigurations payPalConfig = new()
//    {
//        ApiAddress = "https://api-m.sandbox.paypal.com",
//        PayUrl = "",
//        ClientId = "AfxzjPrL2vgk8FJIZdWDFlZov-k-zrL0AcqdgNkYRCX4ZNmFSgG190hFdA-cGtAB7MKWwHHdfq7sNlEf",
//        Secret = "EB1Xw8-lZsoSZ22gCz--2lbrBBxgOi2uTNwZDC6PwcrSLBA1ABmFuFkA5j3ffnKkRRKScTQZujjk30SI"
//    };
//    public static IPaymentGatewayProvider GetSampleProvider(IServiceProvider services)
//    {
//        var payPal = true;
//        IPaymentGatewayProvider provider;

//        if (payPal)
//        {
//            var payPalProviderType = typeof(PayPalPaymentProvider).FullName;
//            provider = services.GetRequiredKeyedService<IPaymentGatewayProvider>(payPalProviderType);
//            provider.Configure(JsonSerializer.Serialize(payPalConfig));
//        }
//        else
//        {
//            var ProviderType = typeof(ZarinPalPaymentProvider).FullName;
//            provider = services.GetRequiredKeyedService<IPaymentGatewayProvider>(ProviderType);
//            provider.Configure(JsonSerializer.Serialize(zarinPalConfig));
//        }

//        return provider;
//    }
//}

//public static class PaymentStorage
//{
//    private static List<ReceiptRequest> List = new List<ReceiptRequest>();

//    internal static ReceiptRequest Create(decimal amount, string currency)
//    {
//        var newPayment = new ReceiptRequest()
//        {
//            Id = DateTime.Now.Ticks,
//            Amount = amount,
//            Currency = currency,// "IRR",
//            Issuer = null,
//            IssuerId = null,
//            Description = "خرید گوشی اینترنتی",
//            AdditionalData = "{}",
//        };

//        List.Add(newPayment);

//        return newPayment;
//    }

//    internal static ReceiptRequestGatewayPayment CreateGatewayPayment(long paymentRequestId)
//    {
//        var paymentRequest = List.First(x => x.Id == paymentRequestId);

//        // اگر قبض فعال نداشته باشد
//        // قبض جدید ساخته می شود

//        var newGatewayPayment = new ReceiptRequestGatewayPayment
//        {
//            Id = DateTime.Now.Ticks,
//            Amount = paymentRequest.Amount,
//            Currency = paymentRequest.Currency,
//            Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.New,
//        };

//        paymentRequest.GatewayPayments.Add(newGatewayPayment);

//        return newGatewayPayment;
//    }
//    internal static void SetCreateProviderRestul(long gatewayPaymentId, bool success, string? createReference)
//    {
//        var paymentRequest = List.Where(c => c.GatewayPayments.Any(c => c.Id == gatewayPaymentId))
//            .First();

//        var gatewayPayment = paymentRequest.GatewayPayments.First(c => c.Id == gatewayPaymentId);

//        if (success)
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Waiting;
//            gatewayPayment.CreateReference = createReference;
//            gatewayPayment.RedirectAt = DateTimeOffset.Now;
//        }
//        else
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Failed;
//            gatewayPayment.CreateReference = createReference;
//            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentGatewayFailedReason.CreateFailed;
//        }
//    }

//    internal static ReceiptRequestGatewayPayment? GetForVerify(long? uniqueRequestId, string? createToken,
//        bool success, string? error)
//    {
//        var query = List.SelectMany(c => c.GatewayPayments).AsQueryable();

//        if (uniqueRequestId.HasValue)
//        {
//            query = query.Where(c => c.Id == uniqueRequestId.Value);
//        }
//        else if (!string.IsNullOrEmpty(createToken))
//        {
//            query = query.Where(c => c.CreateReference == createToken);
//        }
//        else
//        {
//            throw new ArgumentNullException();
//        }
//        var gatewayPayment = query.FirstOrDefault();

//        if (gatewayPayment == null)
//            return null;

//        gatewayPayment.CallBackAt = DateTimeOffset.Now;

//        if (success)
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Settlement;
//        }
//        else
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Failed;
//            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentGatewayFailedReason.CallBackFailed;
//        }

//        return gatewayPayment;
//    }

//    internal static void SaveVerify(VerifyResult verifyResult, ReceiptRequestGatewayPayment gatewayPayment)
//    {
//        if (verifyResult.Success)
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Success;
//            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentGatewayFailedReason.None;

//            if (verifyResult.SupplementaryPaymentInformation is not null)
//            {
//                gatewayPayment.TrackingNumber = verifyResult.SupplementaryPaymentInformation?.TrackingNumber;
//                gatewayPayment.ReferenceRetrievalNumber = verifyResult.SupplementaryPaymentInformation?.ReferenceRetrievalNumber;
//                gatewayPayment.Pan = verifyResult.SupplementaryPaymentInformation?.Pan;
//                gatewayPayment.SuccessReference = verifyResult.SupplementaryPaymentInformation?.SuccessReference;
//                gatewayPayment.MerchantId = verifyResult.SupplementaryPaymentInformation?.MerchantId;
//                gatewayPayment.TerminalId = verifyResult.SupplementaryPaymentInformation?.TerminalId;
//            }
//        }
//        else
//        {
//            gatewayPayment.Status = Honamic.PayMaster.Enums.PaymentGatewayStatus.Failed;
//            gatewayPayment.FailedReason = verifyResult.PaymentFailedReason ?? Honamic.PayMaster.Enums.PaymentGatewayFailedReason.Other;
//        }

//    }
//}
