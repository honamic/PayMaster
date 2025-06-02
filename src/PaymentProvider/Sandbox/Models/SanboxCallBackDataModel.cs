namespace Honamic.PayMaster.PaymentProvider.Sandbox.Models;

public class SanboxCallBackDataModel
{
    public string? Status { get; set; }

    public string? PayId { get; set; }

    public decimal Amount { get; set; }
    
    public string? Currency { get; set; }

    public string? Pan { get; set; }

    public string? Token { get; set; }

    public string? PayRequestId { get;  set; }

    public string? TrackingNumber { get;  set; }
}

