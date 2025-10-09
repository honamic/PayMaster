namespace Honamic.PayMaster.PaymentProviders;

public interface IPaymentGatewayProviderFactory
{
    IPaymentGatewayProvider Create(string ProviderType, string providerConfiguration);
    IPaymentGatewayProvider CreateByDefaultConfiguration(string ProviderType);
    List<KeyValuePair<string, string>> Providers();
}