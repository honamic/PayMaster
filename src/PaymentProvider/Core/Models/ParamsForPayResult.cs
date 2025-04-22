namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class ParamsForPayResult
{
    public ParamsForPayResult()
    {
        LogData = new PaymentProviderLogData();
        PayParams = new Dictionary<string, string>();
    }

    public bool Success { get; set; }
    
    public string? Error { get; set; }
    
    public string? Token { get; set; }

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