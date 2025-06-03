namespace Honamic.PayMaster.PaymentProviders.Models;

public class ValidationConfigurationResult
{
    public ValidationConfigurationResult()
    {
        ValidationErrors = [];
    }
    public bool Success { get; set; }
    public string[] ValidationErrors { get; set; }
}
