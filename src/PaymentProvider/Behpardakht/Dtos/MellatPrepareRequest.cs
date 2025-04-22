
namespace Honamic.PayMaster.PaymentProvider.Behpardakht.Dtos;
public class MellatPrepareRequest
{
    public required string MerchantId { get; set; }
    public decimal Amount { get; set; }
    public required string CallbackUrl { get; set; }
    public required string OrderId { get; set; }
}
