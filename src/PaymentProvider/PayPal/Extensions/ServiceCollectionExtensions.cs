using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProvider.PayPal.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPayPalPaymentProviderServices(this IServiceCollection services)
    {
        AddHttpServices(services);

        services.RegisterPaymentProvider<PayPalPaymentProvider>();

        return services;
    }
    private static void AddHttpServices(IServiceCollection services)
    {
        services.AddTransient<HttpInterceptorService>();
        services.AddHttpClient(Constants.HttpClientName)
            .AddHttpMessageHandler<HttpInterceptorService>();
    }
}