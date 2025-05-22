using Honamic.PayMaster.Extensions;

namespace Honamic.PayMaster.PaymentProviders.Models;

public class PaymentProviderLogData
{
    public string? Url { get; private set; }

    public object? Request { get; private set; }

    public object? Response { get; private set; }

    public object? Exception { get; private set; }

    public string? Message { get; private set; }

    public DateTimeOffset? StartAt { get; private set; }

    public DateTimeOffset? EndAt { get; private set; }

    public void Start(object? request, string? url)
    {
        Url = url;
        Request = request;
        StartAt = DateTimeOffset.Now;
    }

    public void End(DateTimeOffset? dateTime = null)
    {
        EndAt = dateTime ?? DateTimeOffset.Now;
    }

    public void SetResponse(object? response)
    {
        EndAt ??= DateTimeOffset.Now;
        Response = response;
    }

    public void SetException(Exception exception)
    {
        Exception = exception.ExceptionToExceptionInfo();
        Message = exception.Message;
        EndAt ??= DateTimeOffset.Now;
    }

    public void SetMessage(string? message)
    {
        Message = message;
    }
}