using Honamic.PayMaster.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSandboxPaymentProviderServices(this IServiceCollection services)
    {
        services.RegisterPaymentProvider<SandboxPaymentProvider>();

        return services;
    }
}