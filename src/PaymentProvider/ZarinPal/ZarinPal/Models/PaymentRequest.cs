namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentRequest
{
    public string merchant_id { get; set; }
    public decimal amount { get; set; }
    public string callback_url { get; set; }
    public string? description { get; set; }

    /// <summary>
    /// تعیین واحد پولی ریال (IRR) یا تومان(IRT)
    /// </summary>
    public string? currency { get; set; }
    public string? order_id { get; set; }

    public PaymentRequestMetaData? MetaData { get; set; }
}

public class PaymentRequestMetaData
{
    public string? mobile { get; set; }
    public string? email { get; set; }
    public string? card_pan { get; set; }
}
