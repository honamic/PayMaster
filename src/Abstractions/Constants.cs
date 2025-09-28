namespace Honamic.PayMaster;

public static class Constants
{
    public const string Schema = "PayMaster";

    public static class Parameters
    {
        public const string ReceiptRequestIdParameter = "{ReceiptRequestId}";
        public const string ReceiptRequestIssuerReferenceParameter = "{IssuerReference}";
        public const string ReceiptIssuerCodeParameter = "{IssuerCode}";
        public const string GatewayProfileIdParameter = "{GatewayProfileId}";
        public const string GatewayPaymentIdParameter = "{GatewayPaymentId}";
        public const string GatewayPaymentStatusParameter = "{Status}";
    }
}