using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.PaymentProviders;

public interface IPaymentGatewayProvider
{
    void ParseConfiguration(string providerJsonConfiguration);

    public Task<CreateResult> CreateAsync(CreateRequest request);

    public ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);

    public Task<VerifyResult> VerifyAsync(VerifyRequest request);

    TimeSpan GetCallbackValidityDuration();
}