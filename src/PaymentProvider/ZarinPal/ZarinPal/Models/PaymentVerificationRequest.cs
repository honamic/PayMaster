namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentVerificationRequest
{
    public string merchant_id { get; set; }
    public decimal amount { get; set; }
    public string authority { get; set; }
}
