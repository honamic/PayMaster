using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.PaymentProviders.Models;

public class VerifyResult
{
    public VerifyResult()
    {
        VerifyLogData = new PaymentProviderLogData();
    }

    public bool Success { get; set; }

    public string? Error { get; set; }

    public PaymentProviderLogData VerifyLogData { get; }

    public PaymentProviderLogData? SettlementLogData { get; private set; }

    public SupplementaryPaymentInformation? SupplementaryPaymentInformation { get; set; }

    public PaymentGatewayFailedReason? PaymentFailedReason { get; set; }

    public void StartSettlement()
    {
        SettlementLogData = new PaymentProviderLogData();
    }

}
