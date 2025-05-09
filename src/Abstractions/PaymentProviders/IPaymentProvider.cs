using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.PaymentProviders;

public interface IPaymentProvider
{
    void Configure(string providerJsonConfiguration);

    public Task<CreateResult> CreateAsync(CreateRequest request);
    
    public ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);

    public Task<VerfiyResult> VerifyAsync(VerifyRequest request);

}