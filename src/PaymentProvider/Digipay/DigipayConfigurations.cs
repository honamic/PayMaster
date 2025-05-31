namespace Honamic.PayMaster.PaymentProvider.Digipay;

public class DigipayConfigurations
{
    public DigipayConfigurations()
    {
        //https://api.mydigipay.com/digipay/api
        ApiAddress = "https://uat.mydigipay.info/digipay/api"; 
        UserName = "UserName";
        Password= "Password";
    }

    public string ApiAddress { get; set; } 
    
    public string UserName { get; set; }
    
    public string Password { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
}
