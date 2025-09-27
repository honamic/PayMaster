using Honamic.Framework.Domain;
using Honamic.Framework.Endpoints.Web.Extensions;
using Honamic.PayMaster.Options;
using Honamic.PayMaster.PaymentProvider.Behpardakht.Extensions;
using Honamic.PayMaster.PaymentProvider.Digipay.Extensions;
using Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
using Honamic.PayMaster.Web.Extensions;
using Honamic.PayMaster.Wrapper.Extensions;
using Microsoft.EntityFrameworkCore;
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


        builder.Services.AddPayMasterServices(sqlServerConnection);

        builder.Services.AddDefaultUserContextService();
        builder.Services.AddScoped<IAuthorization, DefaultAuthorization>();

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
        DatabaseInitializer.InitializeDatabaseDefaults(app);

        app.Run();
    }
}