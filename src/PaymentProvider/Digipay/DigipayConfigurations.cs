using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Digipay;

public class DigipayConfigurations: IPaymentGatewayProviderConfiguration
{
    public DigipayConfigurations()
    {
        SetDefaultConfiguration();
    }

    public string ApiAddress { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

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
        else if ( !Uri.TryCreate(ApiAddress, UriKind.Absolute, out _))
        {
            errors.Add("ApiAddress is not a valid URL.");
        }

        return errors;
    }
}