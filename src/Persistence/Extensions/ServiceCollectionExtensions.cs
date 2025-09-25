using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Persistence.PaymentGatewayProviders;
using Honamic.PayMaster.Persistence.ReceiptIssuers;
using Honamic.PayMaster.Persistence.ReceiptRequests;
using Honamic.Framework.EntityFramework.Persistence.Extensions;

namespace Honamic.PayMaster.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistenceEntityFrameworkServices(this IServiceCollection services, string connectionString)
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