
using Honamic.PayMaster.PaymentProvider.Core.Models;

namespace Honamic.PayMaster.PaymentProvider.Core;

public interface IPaymentProvider
{
    public Task<CreateResult> ParamsForPayAsync(CreateRequest request);
    
    public ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);

    public Task<VerfiyResult> VerifyAsync(VerifyRequest request);

}