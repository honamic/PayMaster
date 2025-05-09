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

    var createResult = await provider.CreateAsync(new CreateRequest()
    {
        Amount = newPayment.Amount,
        UniqueRequestId = newPayment.UniqueRequestId,
        CallbackUrl = callbackUrl,
    });

    if (createResult.Success)
    {
        PaymentStorage.UpdaeToken(newPayment.UniqueRequestId, createResult.CreateToken);

        newPayment.CreateToken = createResult.CreateToken;

        var redirectUrl = new Uri(createResult.PayUrl!);

        if (createResult.PayVerb == PayVerb.Get)
            foreach (var param in createResult.PayParams)
            {
                redirectUrl = new Uri(redirectUrl + $"?{param.Key}={param.Value}");
            }
        //todo add param to pay url 
        return Results.Redirect(redirectUrl.ToString());
    }

    return Results.Ok(createResult);
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

    if (!ExtractCallBackDataResult.Success)
    {
        return Results.Ok(ExtractCallBackDataResult);
    }

    var dbPayment = PaymentStorage.Get(ExtractCallBackDataResult.UniqueRequestId, ExtractCallBackDataResult.CreateToken);

    VerifyRequest verifyRequest = new()
    {
        PatmentInfo = new VerifyRequestPatmentInfo
        {
            Amount = dbPayment!.Amount,
            UniqueRequestId = dbPayment.UniqueRequestId,
            CreateToken = dbPayment.CreateToken,
        },
        CallBackData = ExtractCallBackDataResult.CallBack
    };

    var verifyResult = await provider.VerifyAsync(verifyRequest);

    return Results.Ok(verifyResult);
});

app.Run();


public static class PaymentFacoty
{
    private static ZarinPalConfigurations zarinPalConfig = new()
    {
        ApiAddress = "https://sandbox.zarinpal.com/",
        PayUrl= "https://sandbox.zarinpal.com/pg/StartPay/",
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
    private static List<VerifyRequestPatmentInfo> List = new List<VerifyRequestPatmentInfo>();

    internal static VerifyRequestPatmentInfo Create(decimal amount)
    {
        var newPayment = new VerifyRequestPatmentInfo()
        {
            UniqueRequestId = DateTime.Now.Ticks,
            Amount = amount
        };

        List.Add(newPayment);

        return newPayment;
    }

    internal static VerifyRequestPatmentInfo? Get(long? uniqueRequestId, string? createToken)
    {
        var query = List.AsQueryable();

        if (uniqueRequestId.HasValue)
        {
            query = query.Where(c => c.UniqueRequestId == uniqueRequestId.Value);
        }
        else if (!string.IsNullOrEmpty(createToken))
        {
            query = query.Where(c => c.CreateToken == createToken);
        }
        else
        {
            throw new ArgumentNullException();
        }

        return query.FirstOrDefault();
    }

    internal static void UpdaeToken(long uniqueRequestId, string? createToken)
    {
        var pay = List.FirstOrDefault(c => c.UniqueRequestId == uniqueRequestId);

        if (pay is not null)
        {
            pay.CreateToken = createToken;
        }
    }
}