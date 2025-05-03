using Honamic.PayMaster.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZarinPalPaymentProviderServices(this IServiceCollection services)
    {
        services.RegisterPaymentProvider<ZarinPalPaymentProvider>();

        return services;
    }
}