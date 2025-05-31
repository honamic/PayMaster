namespace Honamic.PayMaster.PaymentProvider.DigiPay;

public static class Constants
{
    public const string HttpClientName = "DidgipayPaymentProvider";
    public const string DigiPayAuthPath = "/oauth/token";
    public const string CreatePath = "/tickets/business";
    public const string VerifyPath = "/purchases/verify";
    public const string DigiPayRequestOptionsKey = "DigiPayRequestOptionsKey";
    public const string DigiPayVersion = "2022-02-02";
    public const string DigiPayAgent = "WEB";

}