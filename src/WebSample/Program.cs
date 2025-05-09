using Honamic.PayMaster.Core.PaymentRequests;
using Honamic.PayMaster.PaymentProvider.ZarinPal;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Extensions;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using System.Dynamic;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddZarinPalPaymentProviderServices();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/Payment/create/", async (HttpContext context, IServiceProvider services, decimal amount) =>
{
    var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/Payment/callback/providerSmapleId";

    IPaymentProvider provider = PaymentFacoty.GetSampleProvider(services);

    var newPayment = PaymentStorage.Create(amount);

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
            CreateReference = gatewayPayment.GatewayCreateReference,
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

app.Run();


public static class PaymentFacoty
{
    private static ZarinPalConfigurations zarinPalConfig = new()
    {
        ApiAddress = "https://sandbox.zarinpal.com/",
        PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
        MerchantId = "3614255c-8e1a-4729-90d8-92f4119a6489",
    };
    public static IPaymentProvider GetSampleProvider(IServiceProvider services)
    {
        var ProviderType = typeof(ZarinPalPaymentProvider).FullName;

        var provider = services.GetRequiredKeyedService<IPaymentProvider>(ProviderType);
        provider.Configure(JsonSerializer.Serialize(zarinPalConfig));
        return provider;
    }
}

public static class PaymentStorage
{
    private static List<PaymentRequest> List = new List<PaymentRequest>();

    internal static PaymentRequest Create(decimal amount)
    {
        var newPayment = new PaymentRequest()
        {
            Id = DateTime.Now.Ticks,
            Amount = amount,
            Currency = "IRR",
            Requester = null,
            RequesterRef = null,
            Description = "خرید گوشی اینترنتی",
            AdditionalData = "{}",
        };

        List.Add(newPayment);

        return newPayment;
    }

    internal static PaymentRequestPaymentGateway CreateGatewayPayment(long paymentRequestId)
    {
        var paymentRequest = List.First(x => x.Id == paymentRequestId);

        // اگر قبض فعال نداشته باشد
        // قبض جدید ساخته می شود

        var newGatewayPayment = new PaymentRequestPaymentGateway
        {
            Id = DateTime.Now.Ticks,
            Amount = paymentRequest.Amount,
            Currency = paymentRequest.Currency,
            Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.New,
        };

        paymentRequest.GatewayPayments.Add(newGatewayPayment);

        return newGatewayPayment;
    }
    internal static void SetCreateProviderRestul(long gatewayPaymentId, bool success, string? createReference)
    {
        var paymentRequest = List.Where(c => c.GatewayPayments.Any(c => c.Id == gatewayPaymentId))
            .First();

        var gatewayPayment = paymentRequest.GatewayPayments.First(c => c.Id == gatewayPaymentId);

        if (success)
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Waiting;
            gatewayPayment.GatewayCreateReference = createReference;
            gatewayPayment.GatewayRedirectAt = DateTimeOffset.Now;
        }
        else
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Failed;
            gatewayPayment.GatewayCreateReference = createReference;
            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentFailedReason.CreateFailed;
        }
    }

    internal static PaymentRequestPaymentGateway? GetForVerify(long? uniqueRequestId, string? createToken,
        bool success, string? error)
    {
        var query = List.SelectMany(c => c.GatewayPayments).AsQueryable();

        if (uniqueRequestId.HasValue)
        {
            query = query.Where(c => c.Id == uniqueRequestId.Value);
        }
        else if (!string.IsNullOrEmpty(createToken))
        {
            query = query.Where(c => c.GatewayCreateReference == createToken);
        }
        else
        {
            throw new ArgumentNullException();
        }
        var gatewayPayment = query.FirstOrDefault();

        if (gatewayPayment == null)
            return null;

        gatewayPayment.GatewayCallBackAt = DateTimeOffset.Now;

        if (success)
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Settlement;
        }
        else
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Failed;
            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentFailedReason.CallBackFailed;
        }

        return gatewayPayment;
    }

    internal static void SaveVerify(VerfiyResult verifyResult, PaymentRequestPaymentGateway gatewayPayment)
    {
        if (verifyResult.Success)
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Success;
            gatewayPayment.FailedReason = Honamic.PayMaster.Enums.PaymentFailedReason.None;

            if (verifyResult.SupplementaryPaymentInformation is not null)
            {
                gatewayPayment.TrackingNumber = verifyResult.SupplementaryPaymentInformation?.TrackingNumber;
                gatewayPayment.ReferenceRetrievalNumber = verifyResult.SupplementaryPaymentInformation?.ReferenceRetrievalNumber;
                gatewayPayment.Pan = verifyResult.SupplementaryPaymentInformation?.Pan;
                gatewayPayment.GatewaySuccessReference = verifyResult.SupplementaryPaymentInformation?.SuccessReference;
                gatewayPayment.MerchantId = verifyResult.SupplementaryPaymentInformation?.MerchantId;
                gatewayPayment.TerminalId = verifyResult.SupplementaryPaymentInformation?.TerminalId;
            }

        }
        else
        {
            gatewayPayment.Status = Honamic.PayMaster.Enums.GatewayPaymentStatus.Failed;
            gatewayPayment.FailedReason = verifyResult.PaymentFailedReason ?? Honamic.PayMaster.Enums.PaymentFailedReason.Other;
        }

    }
}