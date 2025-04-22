
namespace Honamic.PayMaster.PaymentProvider.Core;

public interface IPaymentProvider
{
    public Task<ParamsForPayResult> ParamsForPayAsync(ParamsForPayRequest request);
}