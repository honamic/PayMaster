using Honamic.Framework.Applications.Extensions;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.PayMaster.Application.Extensions;
using Honamic.PayMaster.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterServices(this IServiceCollection services)
    {
        services.AddDefaultApplicationsServices();
        services.AddSnowflakeIdGeneratorServices();
        
        services.AddPayMasterDomainServices();
        services.AddPayMasterApplicationServices();
    }
}