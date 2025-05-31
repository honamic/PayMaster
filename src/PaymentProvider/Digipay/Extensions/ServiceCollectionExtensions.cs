using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProvider.Digipay.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Digipay.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPayPalPaymentProviderServices(this IServiceCollection services)
    {
        AddHttpServices(services);

        services.RegisterPaymentProvider<DigiPayPaymentProvider>();

        return services;
    }
    private static void AddHttpServices(IServiceCollection services)
    {
        services.AddTransient<HttpInterceptorService>();
        services.AddTransient<IBearerTokensStore, InMemoryBearerTokensStore>();
        services.AddHttpClient(Constants.HttpClientName)
            .AddHttpMessageHandler<HttpInterceptorService>();
    }
}