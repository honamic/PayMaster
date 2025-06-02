namespace Honamic.PayMaster.PaymentProviders.Models;

public class CreateResult
{
    public CreateResult()
    {
        LogData = new PaymentProviderLogData();
        PayParams = new Dictionary<string, string>();
    }

    public bool Success { get; set; }
    
    public string? StatusDescription { get; set; }
    
    public string? CreateReference { get; set; }

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