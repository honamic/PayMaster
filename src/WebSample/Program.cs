using Honamic.PayMaster.PaymentProvider.Core;
using Honamic.PayMaster.PaymentProvider.Core.Models;
using Honamic.PayMaster.PaymentProvider.ZarinPal;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Extensions;
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

    var createResult = await provider.CreateAsync(new Honamic.PayMaster.PaymentProvider.Core.Models.CreateRequest()
    {
        Amount = amount,
        CallbackUrl = callbackUrl,
        UniqueRequestId = DateTime.Now.Millisecond,
    });

    if (createResult.Success)
    {
        var redirectUrl = new Uri(createResult.PayUrl!);

        if (createResult.PayVerb == Honamic.PayMaster.PaymentProvider.Core.Models.PayVerb.Get)
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

    VerifyRequest verifyRequest = new()
    {
        PatmentInfo = new VerifyRequestPatmentInfo
        {
            Amount = 100,
            UniqueRequestId = 123.ToString(),
            CreateToken = "",
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
        MerchantId = "123",
        TerminalId = 456,
        UserName = "UserName",
        Password = "password",
    };
    public static IPaymentProvider GetSampleProvider(IServiceProvider services)
    {
        var ProviderType = typeof(ZarinPalPaymentProvider).FullName;

        var provider = services.GetRequiredKeyedService<IPaymentProvider>(ProviderType);
        provider.Configure(JsonSerializer.Serialize(zarinPalConfig));
        return provider;
    }

}


