namespace Honamic.PayMaster.PaymentProvider.Core;

public class ParamsForPayRequest
{
    public decimal Amount { get; set; }

    //Mellt: CallbackUrl
    //BMI,Saman: RedirectUrl 
    public string CallbackUrl { get; set; }

    //Mellt,BMI: OrderId
    //Saman :ResuNumber
    public string UniqueRequestId { get; set; }

    public string? MobileNumber { get; set; }

    public string? NationalityCode { get; set; }
}