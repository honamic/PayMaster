using Honamic.PayMaster.PaymentProvider.Sandbox.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSandboxWebPaymentProviderServices(this IServiceCollection services)
    {
        services.AddSandboxPaymentProviderServices();

        return services;
    }
}