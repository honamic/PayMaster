namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class ZarinPalResult<T>
{
    public T data { get; set; }
    public object[] errors { get; set; }
}