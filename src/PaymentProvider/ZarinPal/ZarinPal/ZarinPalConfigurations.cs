namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class ZarinPalConfigurations
{
    public ZarinPalConfigurations()
    {
        ApiAddress = "https://api.zarinpal.com/";
        PayUrl = "https://www.zarinpal.com/pg/StartPay/";
        MerchantId = "MerchantId";
    }
    /// <summary>
    /// production: https://api.zarinpal.com/
    /// sanbox: https://sandbox.zarinpal.com/
    /// </summary>
    public string ApiAddress { get; set; }

    /// <summary>
    /// production: https://www.zarinpal.com/pg/StartPay/
    /// sanbox: https://sandbox.zarinpal.com/pg/StartPay/
    /// </summary>
    public string PayUrl { get; set; }

    public string MerchantId { get; set; }
}
