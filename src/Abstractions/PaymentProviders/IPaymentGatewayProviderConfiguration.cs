
namespace Honamic.PayMaster.PaymentProviders;
public interface IPaymentGatewayProviderConfiguration
{
    void SetDefaultConfiguration(bool sandbox = false);

    List<string> IsValid();
}
