using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles;
public class PaymentGatewayProfile : AggregateRoot<long>
{
    public PaymentGatewayProfile()
    {
        JsonConfigurations = "{}";
        ProviderType = "";
    }

    public string Title { get; set; }

    public string Code { get; set; }

    public string ProviderType { get; set; }

    public string? LogoPath { get; set; }

    public string JsonConfigurations { get; set; }

    public bool Enabled { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public static PaymentGatewayProfile CreateAndValidatProvider(CreatePaymentGatewayProfileParameters createParameters, IPaymentGatewayProviderFactory paymentGatewayProviderFactory)
    {
        IPaymentGatewayProvider provider = CreateProvider(createParameters.ProviderType, paymentGatewayProviderFactory);

        ValidationConfiguration(createParameters, provider);

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

    private static void ValidationConfiguration(CreatePaymentGatewayProfileParameters createParameters, IPaymentGatewayProvider provider)
    {
        PaymentProviders.Models.ValidationConfigurationResult ValidationConfigurationResult;

        try
        {
            ValidationConfigurationResult = provider.ValidationConfiguration(createParameters.JsonConfigurations);
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
}