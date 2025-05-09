namespace Honamic.PayMaster.PaymentProviders.Models;

public class VerifyRequest
{
    public required VerifyRequestPatmentInfo PatmentInfo { get; init; }

    public object? CallBackData { get; init; }
}


public class VerifyRequestPatmentInfo
{
    public required long UniqueRequestId { get; init; }

    public required decimal Amount { get; init; }

    public string? CreateReference { get; set; }
}