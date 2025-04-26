namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class ZarinPalResult<T>
{
    public T data { get; set; }
    public ZarinPalErrorResult errors { get; set; }
}


public class ZarinPalErrorResult
{
    public string message { get; set; }
    public int code { get; set; }
    public string[] validations { get; set; }
}
