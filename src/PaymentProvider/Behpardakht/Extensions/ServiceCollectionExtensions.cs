using Honamic.PayMaster.PaymentProvider.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Behpardakht.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBehpardakhtPaymentProviderServices(this IServiceCollection services)
    {
        services.RegisterPaymentProvider<BehpardakhtPaymentProvider>();

        return services;
    }
}