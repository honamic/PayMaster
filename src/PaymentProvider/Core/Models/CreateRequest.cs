namespace Honamic.PayMaster.PaymentProvider.Core.Models;

public class CreateRequest
{
    public required decimal Amount { get; set; }

    public required string CallbackUrl { get; set; }

    public required long UniqueRequestId { get; set; }

    public string? MobileNumber { get; set; }

    public string? NationalityCode { get; set; }

    public string? Email { get; set; }

    public string? Currency { get; set; }

    public string? GatewayNote { get; set; }
}