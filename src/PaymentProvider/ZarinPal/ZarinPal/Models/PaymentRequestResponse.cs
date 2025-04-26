namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentRequestResponse
{
    public string authority { get; set; }
    public int fee { get; set; }
    public string fee_type { get; set; }
    public int code { get; set; }
    public string message { get; set; }
    public string Link { get; set; }
}