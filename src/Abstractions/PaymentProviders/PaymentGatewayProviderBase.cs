using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProviders.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Honamic.PayMaster.PaymentProviders;

public abstract class PaymentGatewayProviderBase : IPaymentGatewayProvider
{
    public abstract void Configure(string providerConfiguration);
    public abstract Task<CreateResult> CreateAsync(CreateRequest prePayRequest);
    public abstract ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);
    public abstract Task<VerifyResult> VerifyAsync(VerifyRequest request);

    protected string ToJson(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            IncludeFields = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        });
    }

    protected string EnumToStringWithDescription(Enum? obj)
    {
        if (obj is null)
            return "[null]";

        var description = obj.GetEnumDescription();

        return $"{obj} | {description}".TrimEnd(' ').TrimEnd('|');
    }

    public virtual TimeSpan GetCallbackValidityDuration()
    {
        return TimeSpan.FromMinutes(30);
    }
}