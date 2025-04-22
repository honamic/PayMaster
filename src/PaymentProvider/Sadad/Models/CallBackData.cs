
namespace Honamic.PayMaster.PaymentProvider.Sadad.Models;

public class CallBackData
{
    public string? OrderId { get; set; }
    
    public string? HashedCardNo { get; set; }
    
    public string? PrimaryAccNo { get; set; }
    
    public string? SwitchResCode { get; set; }
    
    public string? ResCode { get; set; }
    
    public string? CardHolderFullName { get; set; }
    
    public string? Token { get; set; }
}


public class VerifyResultData
{
    public bool Succeed { get; set; }
    public string ResCode { get; set; }
    public string Description { get; set; }
    public string Amount { get; set; }
    public string RetrivalRefNo { get; set; }
    public string SystemTraceNo { get; set; }
    public string OrderId { get; set; }
}