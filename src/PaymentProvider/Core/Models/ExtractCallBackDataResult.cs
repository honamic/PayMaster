namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class ExtractCallBackDataResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public string? UniqueRequestId { get; set; }
    
    public string? Token { get; set; }

    public object? CallBack { get; set; }
}