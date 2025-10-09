using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Digipay;

public class DigipayConfigurations : IPaymentGatewayProviderConfiguration
{
    public DigipayConfigurations()
    {

    }

    public string ApiAddress { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        ApiAddress = "https://api.mydigipay.com/digipay/api";
        ClientId = "YourClientId";
        ClientSecret = "YourClientSecret";
        Password = "YourPassword";
        UserName = "YourUserName";
    }

    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            errors.Add("ClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            errors.Add("ClientSecret is required.");
        }

        if (string.IsNullOrWhiteSpace(UserName))
        {
            errors.Add("UserName is required.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            errors.Add("Password is required.");
        }

        if (string.IsNullOrWhiteSpace(ApiAddress))
        {
            errors.Add("ApiAddress is required.");
        }
        else if (!Uri.TryCreate(ApiAddress, UriKind.Absolute, out _))
        {
            errors.Add("ApiAddress is not a valid URL.");
        }

        return errors;
    }

    public Uri CreateUrl(int type)
    {
        return UriExtensions.Combine(ApiAddress, Constants.CreatePath).AppendQueryParams($"type={type}");
    }

    public Uri VerifyUrl(string trackingCode, int type)
    {
        var path = UriExtensions.JoinUrlSegments(Constants.VerifyPath, trackingCode);

        return UriExtensions.Combine(ApiAddress, path).AppendQueryParams($"type={type}");
    }
}