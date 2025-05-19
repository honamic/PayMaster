namespace Honamic.PayMaster.PaymentProviders;

public interface IPaymentGatewayProviderFactory
{
    IPaymentGatewayProvider Create(string ProviderType, string providerConfiguration);
    List<KeyValuePair<string, string>> Providers();
}