
namespace Honamic.PayMaster.PaymentProvider.PayPal;

public static class Constants
{
    public const string HttpClientName = "PayPalPaymentProvider";
    public const string PayPalAuthPath = "/v1/oauth2/token";
    public const string CheckoutOrdersPath = "/v2/checkout/orders";
    public const string PayPalRequestOptionsKey = "PayPalRequestOptionsKey";
}
