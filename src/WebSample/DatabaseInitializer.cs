using Honamic.PayMaster.Domain.PaymentGatewayProfiles; 
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptIssuers.Parameters;
using Honamic.PayMaster.PaymentProvider.Digipay;
using Honamic.PayMaster.PaymentProvider.Sandbox;
using Honamic.PayMaster.PaymentProvider.ZarinPal;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebSample.Entities;

internal static class DatabaseInitializer
{
    public static void InitializeDatabaseDefaults(this IHost app)
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

            var sandboxProfile= db.Set<PaymentGatewayProfile>().FirstOrDefault(b => b.Code == "sandbox");
            if (sandboxProfile== null)
            {
                SandboxConfigurations sandboxConfig = new()
                {
                    PayUrl = "https://localhost:7777/paymaster/sandbox/pay",
                };

                sandboxProfile= new PaymentGatewayProfile
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

                db.Set<PaymentGatewayProfile>().Add(sandboxProfile);
            }

            var zainpalProfile= db.Set<PaymentGatewayProfile>().FirstOrDefault(b => b.Code == "Default");
            if (zainpalProfile== null)
            {
                ZarinPalConfigurations zarinPalConfig = new()
                {
                    ApiAddress = "https://sandbox.zarinpal.com/",
                    PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
                    MerchantId = "3614255c-8e1a-4729-90d8-92f4119a6489",
                };

                zainpalProfile= new PaymentGatewayProfile
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

                db.Set<PaymentGatewayProfile>().Add(zainpalProfile);
            }


            var digipayProfile= db.Set<PaymentGatewayProfile>().FirstOrDefault(b => b.Code == "digipay");
            if (digipayProfile== null)
            {
                DigipayConfigurations digipayPalConfig = new()
                {
                    ApiAddress = "https://api.mydigipay.com/digipay/api",
                    ClientId = "YourClientId",
                    ClientSecret = "YourClientSecret",
                    Password = "YourPassword",
                    UserName = "YourUserName",
                };

                digipayProfile= new PaymentGatewayProfile
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

                db.Set<PaymentGatewayProfile>().Add(digipayProfile);
            }

            db.SaveChanges();
        }
    }
}