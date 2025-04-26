namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class ZarinPalConfigurations
{
    public ZarinPalConfigurations()
    {
        ApiAddress = "https://api.zarinpal.com/";
        PayUrl = "https://www.zarinpal.com/pg/StartPay/";
        TerminalId = 1234567890;
        MerchantId = "MerchantId";
        UserName = "UserName";
        Password = "Password";
    }

    public long TerminalId { get; set; }

    /// <summary>
    /// production: https://api.zarinpal.com/
    /// sanbox: https://sandbox.zarinpal.com/
    /// </summary>
    public string ApiAddress { get; set; }

    public string PayUrl { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string MerchantId { get; set; }
}
