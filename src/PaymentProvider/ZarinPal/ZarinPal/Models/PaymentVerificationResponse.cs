namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentVerificationResponse
{
    public string wages { get; set; }
    public int code { get; set; }
    public string message { get; set; }
    public string card_hash { get; set; }
    public string card_pan { get; set; }
    public int ref_id { get; set; }
    public string fee_type { get; set; }
    public int fee { get; set; }
    public int shaparak_fee { get; set; }
    public string? order_id { get; set; }
}

