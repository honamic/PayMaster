
namespace Honamic.PayMaster.PaymentProvider.Behpardakht;

public class BehpardakhtConfigurations
{
    public BehpardakhtConfigurations()
    {
        ApiAddress = "https://bpm.shaparak.ir";
        PayUrl = "https://bpm.shaparak.ir/pgwchannel/startpay.mellat";
        TerminalId = "YourTerminalId";
        UserName = "UserName";
        Password= "Password";
    }

    public string TerminalId { get; set; }

    public string ApiAddress { get; set; }

    public string PayUrl { get; set; }
    
    public string UserName { get; set; }
    
    public string Password { get; set; }
}
