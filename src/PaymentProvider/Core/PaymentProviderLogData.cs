using Honamic.PayMaster.PaymentProvider.Core.Extensions;

namespace Honamic.PayMaster.PaymentProvider.Core;

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