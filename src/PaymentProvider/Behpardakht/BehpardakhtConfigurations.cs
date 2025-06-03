using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Behpardakht;

public class BehpardakhtConfigurations : IPaymentGatewayProviderConfiguration
{
    public BehpardakhtConfigurations()
    {
        SetDefaultConfiguration();
    }

    public long TerminalId { get; set; }

    public string ApiAddress { get; set; }

    public string PayUrl { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        TerminalId = 12345678;
        UserName = "YourUsername";
        Password = "YourPassword";

        ApiAddress = sandbox
            ? "https://pgwstest.bpm.bankmellat.ir/pgwchannel/services/pgw"
            : "https://bpm.shaparak.ir/pgwchannel/services/pgw";

        PayUrl = sandbox
            ? "https://pgwstest.bpm.bankmellat.ir/pgwchannel/startpay.mellat"
            : "https://bpm.shaparak.ir/pgwchannel/startpay.mellat";
    }

    public List<string> IsValid()
    {
        var errors = new List<string>();

        if (TerminalId <= 0)
        {
            errors.Add("TerminalId is required and must be greater than zero.");
        }

        if (string.IsNullOrEmpty(ApiAddress))
        {
            errors.Add("ApiAddress is required.");
        }
        else if (!Uri.IsWellFormedUriString(ApiAddress, UriKind.Absolute))
        {
            errors.Add("ApiAddress is not a valid URI.");
        }

        if (string.IsNullOrEmpty(PayUrl))
        {
            errors.Add("PayUrl is required.");
        }
        else if (!Uri.IsWellFormedUriString(PayUrl, UriKind.Absolute))
        {
            errors.Add("PayUrl is not a valid URI.");
        }

        if (string.IsNullOrEmpty(UserName))
        {
            errors.Add("UserName is required.");
        }

        if (string.IsNullOrEmpty(Password))
        {
            errors.Add("Password is required.");
        }

        return errors;
    }
}
