
using Honamic.PayMaster.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProviders;

public class PaymentGatewayProviderFactory : IPaymentGatewayProviderFactory
{
    private readonly IServiceProvider _services;

    public PaymentGatewayProviderFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IPaymentGatewayProvider Create(string ProviderType, string providerConfiguration)
    {
        var provider = _services.GetRequiredKeyedService<IPaymentGatewayProvider>(ProviderType);

        provider.ParseConfiguration(providerConfiguration);

        return provider;
    }

    public List<KeyValuePair<string, string>> Providers()
    {
        return ServiceCollectionExtensions.Providers;
    }
}