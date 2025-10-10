using Honamic.Framework.Domain;
using Honamic.Framework.Endpoints.Web.Authorization;
using Honamic.Framework.Endpoints.Web.Extensions;
using Honamic.PayMaster.PaymentProvider.Behpardakht.Extensions;
using Honamic.PayMaster.PaymentProvider.Digipay.Extensions;
using Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
using Honamic.PayMaster.WebApi.Extensions;
using Honamic.PayMaster.WebApi.Management.Extensions;
using Honamic.PayMaster.Wrapper.Extensions;
using Microsoft.EntityFrameworkCore;
using WebSample;
using WebSample.Entities;
using Honamic.Framework.Persistence.EntityFramework.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddRazorPages();
        builder.Services.AddSwaggerGen();


        var sqlServerConnection = builder.Configuration.GetConnectionString("SqlServerConnectionString");
        builder.Services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(sqlServerConnection);
            options.AddPersianYeKeCommandInterceptor();
            options.AddAuditFieldsSaveChangesInterceptor(serviceProvider,
                Honamic.Framework.EntityFramework.Interceptors.AuditFields.AuditType.UserNameAndId);
            options.AddAggregateRootVersionInterceptor(serviceProvider);
            options.AddMarkAsDeletedInterceptors();
        });

        builder.Services.AddPayMasterWrapper(option =>
        {
            option.UseEntityFrameWorkPersistence<SampleDbContext>();
            // option.UseSqlServerPersistence(sqlServerConnection);

            option.UseSqlServerQueryModel(sqlServerConnection!);
            // option.UseEntityFrameWorkQueryModel<SampleQueryDbContext>();

            option.Configure(config =>
            {
                config.CallBackUrl = "https://localhost:7777/api/paymaster/callback/{ReceiptRequestId}/{GatewayPaymentId}/";
                config.SupportedCurrencies = ["IRR", "USD"];
            });

            option.Services.AddAllPaymentProviderServices();
            option.Services.AddDigipayPaymentProviderServices();
            option.Services.AddSandboxWebPaymentProviderServices();

            builder.Services.AddDefaultUserContextService<DefaultUserContext>();
            builder.Services.AddScoped<IAuthorization, DefaultAuthorization>();
        });


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.MapRazorPages();

        app.InitializeDatabaseDefaults();

        app.MapPayMasterEndpoints();
        app.UseSandboxPayEndpoints();
        app.MapPaymentEndpoints();
        app.MapPayMasterManagementEndpoints();


        app.Run();
    }
}