namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class VerifyRequest
{
    public required VerifyRequestPatmentInfo PatmentInfo { get; init; }

    public object? CallBackData { get; init; }
}


public class VerifyRequestPatmentInfo
{
    public required string UniqueRequestId { get; init; }

    public decimal Amount { get; init; }

    public string? CreateToken { get; set; }
}