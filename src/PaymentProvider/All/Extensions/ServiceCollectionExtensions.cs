using Honamic.PayMaster.PaymentProvider.PayPal.Extensions;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Behpardakht.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllPaymentProviderServices(this IServiceCollection services)
    {
        services.AddZarinPalPaymentProviderServices();
        services.AddPayPalPaymentProviderServices();

        return services;
    }
}