using Honamic.Framework.Domain;
using Honamic.Framework.Endpoints.Web.Extensions;
using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptIssuers.Parameters;
using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.Options;
using Honamic.PayMaster.PaymentProvider.Behpardakht.Extensions;
using Honamic.PayMaster.PaymentProvider.Digipay;
using Honamic.PayMaster.PaymentProvider.Digipay.Extensions;
using Honamic.PayMaster.PaymentProvider.Sandbox;
using Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
using Honamic.PayMaster.PaymentProvider.ZarinPal;
using Honamic.PayMaster.Persistence.Extensions;
using Honamic.PayMaster.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebSample;
using WebSample.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAllPaymentProviderServices();
        builder.Services.AddDigipayPaymentProviderServices();
        builder.Services.AddSandboxWebPaymentProviderServices();
        builder.Services.AddHttpClient();

        var sqlServerConnection = builder.Configuration.GetConnectionString("SqlServerConnectionString");
        builder.Services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(sqlServerConnection);
        });


        builder.Services.AddPayMasterServices();
        builder.Services.AddDefaultUserContextService();
        builder.Services.AddScoped<IAuthorization, DefaultAuthorization>();

        builder.Services.AddPersistenceEntityFrameworkServices(sqlServerConnection);

        builder.Services.Configure<PayMasterOptions>(c =>
        {
            c.CallBackUrl = "https://localhost:7777/Payments/callback/{ReceiptRequestId}/{GatewayPaymentId}/";
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UsePayMasterEndpoints();
        app.UseSandboxPayEndpoints();

        PaymentEndpoints.MapPaymentEndpoints(app);

        InitializeDatabaseDefaults(app);

        app.Run();
    }

    private static void InitializeDatabaseDefaults(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
            db.Database.Migrate();

            var defaultIssuer = db.Set<ReceiptIssuer>().FirstOrDefault(b => b.Code == "Default");
            if (defaultIssuer == null)
            {
                var parameters = new ReceiptIssuerParameters
                {
                    Id = 100,
                    Code = "Default",
                    CallbackUrl = "/Payment/{ReceiptRequestId}/{Status}",
                    Title = "صادر کننده پیش فرض",
                    Description = "تست",
                    Enabled = true,
                };

                defaultIssuer = ReceiptIssuer.Create(parameters);

                db.Set<ReceiptIssuer>().Add(defaultIssuer!);
            }

            var sandboxProvider = db.Set<PaymentGatewayProvider>().FirstOrDefault(b => b.Code == "sandbox");
            if (sandboxProvider == null)
            {
                SandboxConfigurations sandboxConfig = new()
                {
                    PayUrl = "https://localhost:7777/paymaster/sandbox/pay",
                };

                sandboxProvider = new PaymentGatewayProvider
                {
                    Id = 1000,
                    Code = "sandbox",
                    Title = "تست پرداخت",
                    Enabled = true,
                    JsonConfigurations = JsonSerializer.Serialize(sandboxConfig),
                    MinimumAmount = 1000,
                    MaximumAmount = null,
                    ProviderType = typeof(SandboxPaymentProvider).FullName,
                    LogoPath = null,
                };

                db.Set<PaymentGatewayProvider>().Add(sandboxProvider);
            }

            var zainpalProvider = db.Set<PaymentGatewayProvider>().FirstOrDefault(b => b.Code == "Default");
            if (zainpalProvider == null)
            {
                ZarinPalConfigurations zarinPalConfig = new()
                {
                    ApiAddress = "https://sandbox.zarinpal.com/",
                    PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
                    MerchantId = "3614255c-8e1a-4729-90d8-92f4119a6489",
                };

                zainpalProvider = new PaymentGatewayProvider
                {
                    Id = 1001,
                    Code = "Default",
                    Title = "زرین پل تستی",
                    Enabled = true,
                    JsonConfigurations = JsonSerializer.Serialize(zarinPalConfig),
                    MinimumAmount = 1000,
                    MaximumAmount = null,
                    ProviderType = typeof(ZarinPalPaymentProvider).FullName,
                    LogoPath = null,
                };

                db.Set<PaymentGatewayProvider>().Add(zainpalProvider);
            }


            var digipayProvider = db.Set<PaymentGatewayProvider>().FirstOrDefault(b => b.Code == "digipay");
            if (digipayProvider == null)
            {
                DigipayConfigurations digipayPalConfig = new()
                {
                    ApiAddress = "https://api.mydigipay.com/digipay/api",
                    ClientId = "YourClientId",
                    ClientSecret = "YourClientSecret",
                    Password = "YourPassword",
                    UserName = "YourUserName",
                };

                digipayProvider = new PaymentGatewayProvider
                {
                    Id = 1002,
                    Code = "digipay",
                    Title = "دیجی پی",
                    Enabled = true,
                    JsonConfigurations = JsonSerializer.Serialize(digipayPalConfig),
                    MinimumAmount = 1000,
                    MaximumAmount = null,
                    ProviderType = typeof(DigiPayPaymentProvider).FullName,
                    LogoPath = null,
                };

                db.Set<PaymentGatewayProvider>().Add(digipayProvider);
            }

            db.SaveChanges();
        }
    }
}