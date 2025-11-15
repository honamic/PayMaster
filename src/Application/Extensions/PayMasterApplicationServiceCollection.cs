using Honamic.Framework.Application.Authorizes;
using Honamic.Framework.Application.Extensions;
using Honamic.PayMaster.Application.PaymentProviders;
using Honamic.PayMaster.Domain.Extensions;
using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Application.Extensions;

public static class PayMasterApplicationServiceCollection
{
    public static IServiceCollection AddPayMasterApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHandlersFromAssemblies();

        DynamicPermissionRegistry.RegisterAssemblies(typeof(PayMasterConstants).Assembly);

        services.AddSingleton<IPaymentGatewayProviderFactory, PaymentGatewayProviderFactory>();
        services.AddSingleton<IBearerTokensStore, InMemoryBearerTokensStore>();

        services.AddPayMasterDomainServices();

        return services;
    }
}