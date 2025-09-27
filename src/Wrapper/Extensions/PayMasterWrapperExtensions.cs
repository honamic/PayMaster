using Honamic.Framework.Applications.Extensions;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.PayMaster.Application.Extensions;
using Honamic.PayMaster.Domain.Extensions;
using Honamic.PayMaster.Persistence.Extensions;
using Honamic.PayMaster.Wrapper.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
namespace Honamic.PayMaster.Wrapper.Extensions;

public static class PayMasterWrapperExtensions
{
    public static void AddPayMasterWrapper(this IServiceCollection services, Action<PayMasterWrapperBuilder> setupAction)
    {
        ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));

        PayMasterWrapperBuilder payMasterWrapperBuilder = new PayMasterWrapperBuilder(services);

        setupAction(payMasterWrapperBuilder);

        services.AddFrameworkServies();
        services.AddPayMasterModuleServices();

        services.AddScoped<IPayMasterFacade, PayMasterFacade>();
    }

    private static void AddPayMasterModuleServices(this IServiceCollection services)
    {
        services.AddPayMasterDomainServices();
        services.AddPayMasterApplicationServices();
        services.AddPayMasterPersistenceServices();
    }

    private static void AddFrameworkServies(this IServiceCollection services)
    {
        services.AddDefaultApplicationsServices();
        services.AddSnowflakeIdGeneratorServices(); 
    }
}