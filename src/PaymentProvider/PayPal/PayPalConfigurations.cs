namespace Honamic.PayMaster.PaymentProvider.PayPal;

public class PayPalConfigurations
{
    public PayPalConfigurations()
    {
        ApiAddress = "https://api-m.sandbox.paypal.com";
        PayUrl = "https://Pay.paypal.com";
        ClientId = "YOUR_CLIENT_ID";
        Secret = "YOUR_SECRET";
        AutoLogin = true;
    }


    public string ApiAddress { get; set; }

    public string PayUrl { get; set; }

    public string ClientId { get; set; }

    public string Secret { get; set; }

    public bool AutoLogin { get; set; }
}
