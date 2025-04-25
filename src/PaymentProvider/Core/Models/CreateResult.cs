namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class CreateResult
{
    public CreateResult()
    {
        LogData = new PaymentProviderLogData();
        PayParams = new Dictionary<string, string>();
    }

    public bool Success { get; set; }
    
    public string? Error { get; set; }
    
    public string? CreateToken { get; set; }

    public Dictionary<string, string> PayParams { get; set; }

    public string? PayUrl { get; set; }

    public PayVerb? PayVerb { get; set; }

    public PaymentProviderLogData LogData { get; }
}

public enum PayVerb
{
    Post,
    Get
}