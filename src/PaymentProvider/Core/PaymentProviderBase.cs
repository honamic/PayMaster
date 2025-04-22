using Honamic.PayMaster.PaymentProvider.Core.Extensions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Honamic.PayMaster.PaymentProvider.Core;

public abstract class PaymentProviderBase : IPaymentProvider
{
    public abstract void Configure(string providerConfiguration);
    public abstract Task<ParamsForPayResult> ParamsForPayAsync(ParamsForPayRequest prePayRequest);

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
}