using Honamic.Framework.Applications.Extensions;
using Honamic.Framework.EntityFramework.Persistence.Extensions;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.PayMaster.Application.Extensions;
using Honamic.PayMaster.Domain.Extensions;
using Honamic.PayMaster.Persistence;
using Honamic.PayMaster.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
namespace Honamic.PayMaster.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterServices(this IServiceCollection services,string connectionString)
    {
        services.AddDefaultApplicationsServices();
        services.AddSnowflakeIdGeneratorServices();
        
        services.AddPayMasterDomainServices();
        services.AddPayMasterApplicationServices();

        services.AddPayMasterPersistenceServices();

        DebuggerConnectionStringLog(connectionString);
        services.AddDbContext<PaymasterDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddPersianYeKeCommandInterceptor();
            options.AddAggregateRootVersionInterceptor(serviceProvider);
            options.AddMarkAsDeletedInterceptors();
        });

        services.AddUnitOfWorkByEntityFramework<PaymasterDbContext>();
    }


    private static void DebuggerConnectionStringLog(string? SqlServerConnection)
    {
#if DEBUG
        Console.WriteLine($"EF connection string:`{SqlServerConnection}`");
#endif

    } 
}