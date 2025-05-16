namespace Honamic.PayMaster.PaymentProviders.Models;

public class SupplementaryPaymentInformation
{
    public string? SuccessReference { get; set; }

    /// <summary>
    /// شماره مرجع 12 رقمی یا RRN
    /// </summary>
    public string? ReferenceRetrievalNumber { get; set; }

    /// <summary>
    /// شماره پیگیری 6 رقمی
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// شماره کارت
    /// </summary>
    public string? Pan { get; set; }

    public string? TerminalId { get; set; }

    public string? MerchantId { get; set; }
}