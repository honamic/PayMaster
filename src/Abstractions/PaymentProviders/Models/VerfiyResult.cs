using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.PaymentProviders.Models;

public class VerfiyResult
{
    public VerfiyResult()
    {
        LogData = new PaymentProviderLogData();
    }

    public bool Success { get; set; }

    public string? Error { get; set; }

    public PaymentProviderLogData LogData { get; }

    public SupplementaryPaymentInformation? SupplementaryPaymentInformation { get; set; }

    public PaymentGatewayFailedReason? PaymentFailedReason { get; set; }
}
