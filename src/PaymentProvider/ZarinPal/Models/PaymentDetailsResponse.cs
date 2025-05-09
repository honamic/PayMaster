namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class PaymentDetailsResponse
{
    public int Code { get; set; }
    public string Message { get; set; }
    public PaymentDetails Data { get; set; }
}