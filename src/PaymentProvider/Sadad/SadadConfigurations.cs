
namespace Honamic.PayMaster.PaymentProvider.Sadad;

public class SadadConfigurations
{
    public SadadConfigurations()
    {
        PaymentRequestUri = "https://sadad.shaparak.ir/api/v0/Request/PaymentRequest";
        PurchasePage = "https://sadad.shaparak.ir/Purchase";
        TerminalId = "YourTerminalId";
        MerchantId = "YourMerchantId";
        MerchantKey = "YourMerchantKey";
    }

    /// <summary>
    /// شماره ترمینال : به منظور تفکیک پايانه های مختلف يک پذيرنده
    /// </summary>
    public string TerminalId { get; set; }

    public string PaymentRequestUri { get; set; }

    public string PurchasePage { get; set; }

    /// <summary>
    /// کلید تراکنش کد مربوط به شناسايی پذيرنده،
    /// الزم به توضیح است اين کد از لحاظ  امنیتی دارای اهمیت بالایی است
    /// به هیچ وجه اين کد نبايستی در اختیار اشخاص ديگر باشد.
    /// </summary>
    public string MerchantKey { get; set; }

    /// <summary>
    /// شماره پذيرنده : شماره يکتا به منظور مشخص شدن پذيرنده
    /// </summary>
    public string MerchantId { get;  set; }
}
