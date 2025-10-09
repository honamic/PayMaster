using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Exceptions;

namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles;
public class PaymentGatewayProfile : AggregateRoot<long>
{
    public PaymentGatewayProfile()
    {
        JsonConfigurations = "{}";
        ProviderType = "";
    }

    public string Title { get; private set; }

    public string Code { get; private set; }

    public string ProviderType { get; private set; }

    public string? LogoPath { get; private set; }

    public string JsonConfigurations { get; private set; }

    public bool Enabled { get; private set; }

    public decimal? MinimumAmount { get; private set; }

    public decimal? MaximumAmount { get; private set; }

    public static PaymentGatewayProfile CreateAndValidatProvider(CreatePaymentGatewayProfileParameters createParameters, IPaymentGatewayProviderFactory paymentGatewayProviderFactory)
    {
        IPaymentGatewayProvider provider = CreateProvider(createParameters.ProviderType, paymentGatewayProviderFactory);

        ValidationConfiguration(createParameters.JsonConfigurations, provider);

        return new PaymentGatewayProfile()
        {
            Id = createParameters.Id,
            Title = createParameters.Title,
            Code = createParameters.Code,
            ProviderType = createParameters.ProviderType,
            LogoPath = createParameters.LogoPath,
            MinimumAmount = createParameters.MinimumAmount,
            MaximumAmount = createParameters.MaximumAmount,
            JsonConfigurations = createParameters.JsonConfigurations,
            Enabled = createParameters.Enabled,
        };
    }

    private static void ValidationConfiguration(string jsonConfigurations, IPaymentGatewayProvider provider)
    {
        PaymentProviders.Models.ValidationConfigurationResult ValidationConfigurationResult;

        try
        {
            ValidationConfigurationResult = provider.ValidationConfiguration(jsonConfigurations);
        }
        catch (Exception ex)
        {
            throw new BusinessException("خطا در خواندن تنظیمات درگاه " + ex.Message);
        }

        if (ValidationConfigurationResult.ValidationErrors.Any())
        {
            var errors = string.Join(',', ValidationConfigurationResult.ValidationErrors);
            throw new BusinessException($"تنظیمات درگاه صحیح نیست" + Environment.NewLine + errors);
        }
    }

    private static IPaymentGatewayProvider CreateProvider(string providerType, IPaymentGatewayProviderFactory paymentGatewayProviderFactory)
    {
        try
        {
            return paymentGatewayProviderFactory.CreateByDefaultConfiguration(providerType);

        }
        catch (PaymentProviderNotFoundException)
        {
            throw new BusinessException($"Payment provider '{providerType}' not found.");
        }
        catch (Exception)
        {
            throw new BusinessException($"create Payment provider '{providerType}' failed.");
        }
    }

    public static PaymentGatewayProfile Create(CreatePaymentGatewayProfileParameters createParameters)
    {
        return new PaymentGatewayProfile()
        {
            Id = createParameters.Id,
            Title = createParameters.Title,
            Code = createParameters.Code,
            ProviderType = createParameters.ProviderType,
            LogoPath = createParameters.LogoPath,
            MinimumAmount = createParameters.MinimumAmount,
            MaximumAmount = createParameters.MaximumAmount,
            JsonConfigurations = createParameters.JsonConfigurations,
            Enabled = createParameters.Enabled,
        };
    }

    public void UpdateAndValidate(UpdatePaymentGatewayProfileParameters updateParameters, IPaymentGatewayProviderFactory paymentGatewayProviderFactory)
    {
        var provider = CreateProvider(ProviderType, paymentGatewayProviderFactory);

        ValidationConfiguration(updateParameters.JsonConfigurations, provider);

        Title = updateParameters.Title;
        Code = updateParameters.Code;
        LogoPath = updateParameters.LogoPath;
        MinimumAmount = updateParameters.MinimumAmount;
        MaximumAmount = updateParameters.MaximumAmount;
        JsonConfigurations = updateParameters.JsonConfigurations;
        Enabled = updateParameters.Enabled;
    }

}