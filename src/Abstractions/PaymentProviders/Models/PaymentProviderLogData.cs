using Honamic.PayMaster.Extensions;

namespace Honamic.PayMaster.PaymentProviders.Models;

public class PaymentProviderLogData
{
    public object? Request { get; set; }

    public object? Response { get; set; }

    public object? Exception { get; private set; }

    public string? Message { get; set; }

    public void SetException(Exception exception)
    {
        Exception = exception.ExceptionToExceptionInfo();
    }
}