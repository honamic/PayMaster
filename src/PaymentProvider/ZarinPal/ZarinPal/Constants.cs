namespace Honamic.PayMaster.PaymentProvider.ZarinPal;

public class Constants
{
    public const string PAYMENT_REQUEST_URL = "/pg/v4/payment/request.json";
    public const string PAYMENT_VERIFICATION_URL = "/pg/v4/payment/verify.json";
    public const string PAYMENT_DETAILS_URL = "/pg/v4/payment/details.json";
    public const string REFUND_URL = "/pg/v4/payment/refund.json";
    
    public const string HttpClientName = "ZarinPalPaymentProvider";
}
