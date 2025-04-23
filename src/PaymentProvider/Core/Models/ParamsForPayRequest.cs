namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class ParamsForPayRequest
{
    public decimal Amount { get; set; }

    public required string CallbackUrl { get; set; }

    public required long UniqueRequestId { get; set; }

    public string? MobileNumber { get; set; }

    public string? NationalityCode { get; set; }
}