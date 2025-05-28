namespace Honamic.PayMaster.Domains.ReceiptRequests.Exceptions; 

/// <summary>
/// Error codes for payment domain exceptions
/// </summary>
public static class PaymentErrorCodes
{
    // Gateway Payment Errors (PGP)
    public const string GatewayPaymentNotFound = "PGP001";
    public const string GatewayProviderNotFound = "PGP002";
    public const string GatewayProviderCreationFailed = "PGP003";
    public const string NoDefaultGatewayProvider = "PGP004";
    public const string SpecifiedGatewayProviderNotFound = "PGP005";
    public const string GatewayProviderDisabled = "PGP006";
    public const string GatewayMinAmountLimit = "PGP007";
    public const string GatewayMaxAmountLimit = "PGP008";
    public const string CreateReferenceExceedsLimit = "PGP009";

    // Verification Errors (VRF)
    public const string AmountMismatch = "VRF001";
    public const string ReferenceIdMismatch = "VRF002";
    public const string RequestIdMismatch = "VRF003";
    public const string InvalidVerificationStatus = "VRF004";
    public const string AuthorityMismatch = "VRF005";
    public const string EmptyCallbackData = "VRF006";
    public const string InvalidPaymentStatus = "VRF007";

    // Receipt Request Errors (RCT)
    public const string InvalidPayment = "RCT001";
    public const string PaymentNotFound = "RCT002";

    // Issuer Errors (ISS)
    public const string NoIssuerSpecified = "ISS001";
    public const string IssuerCodeNotFound = "ISS002";
}
