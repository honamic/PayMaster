using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Extensions;
public static class GatewayPaymentProviderServiceCollectionExtensions
{
    public static List<KeyValuePair<string, string>> Providers = new List<KeyValuePair<string, string>>();

    public static void RegisterPaymentProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IPaymentGatewayProvider
    {
        services.AddKeyedTransient<IPaymentGatewayProvider, TProvider>(typeof(TProvider).FullName);

        var providerName = typeof(TProvider).Name.Replace("PaymentProvider", "", StringComparison.InvariantCultureIgnoreCase);

        Providers.Add(new KeyValuePair<string, string>(typeof(TProvider).FullName!, providerName));
    }
}