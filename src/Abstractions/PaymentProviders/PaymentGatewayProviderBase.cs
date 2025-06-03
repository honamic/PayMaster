using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProviders.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Honamic.PayMaster.PaymentProviders;

public abstract class PaymentGatewayProviderBase<TConfiguration> : IPaymentGatewayProvider
    where TConfiguration : class, IPaymentGatewayProviderConfiguration, new()
{
    private TConfiguration? _configurations;

    protected TConfiguration Configurations =>
        _configurations
        ?? throw new InvalidOperationException("Provider configuration is not set.");

    public abstract Task<CreateResult> CreateAsync(CreateRequest prePayRequest);
    public abstract ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue);
    public abstract Task<VerifyResult> VerifyAsync(VerifyRequest request);

    public virtual void ParseConfiguration(string providerConfiguration)
    {
        var configurations = JsonSerializer.Deserialize<TConfiguration>(providerConfiguration, new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            PropertyNameCaseInsensitive = true
        });

        InternalValidation(configurations);

        _configurations = configurations!;
    }

    public virtual void SetConfiguration(TConfiguration configurations)
    {
        InternalValidation(configurations);
        _configurations = configurations!;
    }

    public string GetDefaultJsonConfigurations(bool sandbox = false)
    {
        var configurations = new TConfiguration();
        configurations.SetDefaultConfiguration(sandbox);
        return ToJson(configurations);
    }

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

    private void InternalValidation(TConfiguration? configurations)
    {
        if (configurations is null)
        {
            throw new ArgumentException("Provider configuration is invalid or empty.");
        }

        var errors = configurations.IsValid();

        if (errors.Count > 0)
            throw new ArgumentException($"Invalid provider configuration: {string.Join(",", errors)}");
    }
}