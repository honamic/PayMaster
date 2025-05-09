namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentDetails
{
    public long Amount { get; set; }
    public string CardHash { get; set; }
    public string CardPan { get; set; }
    public string Status { get; set; }
}