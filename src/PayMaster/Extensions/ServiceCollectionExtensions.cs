using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.Framework.Applications.Extensions;
using Honamic.PayMaster.Persistence;
using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Honamic.PayMaster.Core.ReceiptIssuers;
using Honamic.PayMaster.Core.ReceiptRequests;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;

namespace Honamic.PayMaster.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterServices(this IServiceCollection services, string connectionString)
    {
        services.AddApplicationServices();
        services.AddPersistenceEntityFrameworkServices(connectionString);
        services.AddSnowflakeIdGeneratorServices();
        services.AddDefaultApplicationsServices();
        services.AddBackgroundJobs();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddDefaultApplicationsServices();

        services.AddCommandHandler<CreateReceiptRequestCommand,
            CreateReceiptRequestCommandHandler,
            CreateReceiptRequestCommandResult>();
    }

    private static void AddBackgroundJobs(this IServiceCollection services)
    {

    }

    private static void AddPersistenceEntityFrameworkServices(this IServiceCollection services, string connectionString)
    {
        DebuggerConnectionStringLog(connectionString);
        services.AddDbContext<PaymasterDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddPersianYeKeCommandInterceptor();
            options.AddAggregateRootVersionInterceptor(serviceProvider);
            options.AddMarkAsDeletedInterceptors();
            DebuggerConsoleLog(options);
        });

        //for default DbContext in framework
        services.AddScoped<DbContext>((sp) => sp.GetRequiredService<PaymasterDbContext>());
        services.AddTransient<IPaymentGatewayProviderRepository, PaymentGatewayProviderRepository>();
        services.AddTransient<IReceiptIssuerRepository, ReceiptIssuerRepository>();
        services.AddTransient<IReceiptRequestRepository, ReceiptRequestRepository>();
        services.AddUnitOfWorkByEntityFramework();
    }

    private static void DebuggerConnectionStringLog(string? SqlServerConnection)
    {
#if DEBUG
        Console.WriteLine($"EF connection string:`{SqlServerConnection}`");
#endif

    }

    private static void DebuggerConsoleLog(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(c => Console.WriteLine(c));
#endif
    }
}