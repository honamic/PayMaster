
namespace Honamic.PayMaster.PaymentProvider.Behpardakht.Dtos;

public class CallBackDataModel
{
    public string? ResCode { get; set; }

    public string? SaleOrderId { get; set; }

    public string? RefId { get; set; }

    public string? SaleReferenceId { get; set; }

    public string? CardHolderInfo { get; set; }

    public string? CardHolderPan { get; set; }

    public string? FinalAmount { get; set; }
}
