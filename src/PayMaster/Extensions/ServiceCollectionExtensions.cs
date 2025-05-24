using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.Framework.Applications.Extensions;
using Honamic.PayMaster.Persistence;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
using Honamic.PayMaster.PaymentProviders;
using Honamic.Framework.Applications.Results;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptIssuers;

namespace Honamic.PayMaster.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterServices(this IServiceCollection services, string connectionString)
    {
        services.AddDomainServices();
        services.AddApplicationServices();
        services.AddPersistenceEntityFrameworkServices(connectionString);
        services.AddSnowflakeIdGeneratorServices(); 
        services.AddBackgroundJobs();
    }

    private static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ICreatePaymentDomainService, CreatePaymentDomainService>();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddDefaultApplicationsServices();

        services.AddSingleton<IPaymentGatewayProviderFactory, PaymentGatewayProviderFactory>();


        services.AddCommandHandler<CreateReceiptRequestCommand,
            CreateReceiptRequestCommandHandler,
            Result<CreateReceiptRequestCommandResult>>();

        services.AddCommandHandler<PayReceiptRequestCommand,
            PayReceiptRequestCommandHandler,
            Result<PayReceiptRequestCommandResult>>();

        services.AddCommandHandler<CallBackGatewayPaymentCommand,
            CallBackGatewayPaymentCommandHandler,
            Result<CallBackGatewayPaymentCommandResult>>();
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

        services.AddTransient<IPaymentGatewayProviderRepository, PaymentGatewayProviderRepository>();
        services.AddTransient<IReceiptIssuerRepository, ReceiptIssuerRepository>();
        services.AddTransient<IReceiptRequestRepository, ReceiptRequestRepository>();
        services.AddUnitOfWorkByEntityFramework<PaymasterDbContext>();
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