
using Honamic.PayMaster.PaymentProvider.Core.Models;

namespace Honamic.PayMaster.PaymentProvider.Core;

public interface IPaymentProvider
{
    public Task<ParamsForPayResult> ParamsForPayAsync(ParamsForPayRequest request);
    
    public ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);

    public Task<VerfiyResult> VerifyAsync(VerifyRequest request);

}