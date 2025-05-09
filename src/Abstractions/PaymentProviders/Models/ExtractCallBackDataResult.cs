namespace Honamic.PayMaster.PaymentProviders.Models;

public class ExtractCallBackDataResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public long? UniqueRequestId { get; set; }
    
    public string? CreateToken { get; set; }

    public object? CallBack { get; set; }
}