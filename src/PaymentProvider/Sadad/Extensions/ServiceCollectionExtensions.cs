using Honamic.PayMaster.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Sadad.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSadadPaymentProviderServices(this IServiceCollection services)
    {
        services.RegisterPaymentProvider<SadadPaymentProvider>();

        return services;
    }
}