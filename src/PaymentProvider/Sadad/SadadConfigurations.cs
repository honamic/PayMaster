using Honamic.PayMaster.PaymentProviders;

namespace Honamic.PayMaster.PaymentProvider.Sadad;

public class SadadConfigurations : IPaymentGatewayProviderConfiguration
{
    public SadadConfigurations()
    {
        SetDefaultConfiguration();
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
    public string MerchantId { get; set; }

    public void SetDefaultConfiguration(bool sandbox = false)
    {
        MerchantId = "YourMerchantId";
        MerchantKey = "YourMerchantKey";
        TerminalId = "YourTerminalId";

        PaymentRequestUri = sandbox
        ? "https://sandbox.sadad.shaparak.ir/api/v0/Request/PaymentRequest"
        : "https://sadad.shaparak.ir/api/v0/Request/PaymentRequest";

        PurchasePage = sandbox
        ? "https://sandbox.sadad.shaparak.ir/api/v0/Request/PaymentRequest"
        : "https://sadad.shaparak.ir/api/v0/Request/PaymentRequest";
    }

    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(this.TerminalId))
        {
            errors.Add("TerminalId is required.");
        }

        if (string.IsNullOrEmpty(this.MerchantId))
        {
            errors.Add("MerchantId is required.");
        }

        if (string.IsNullOrEmpty(this.MerchantKey))
        {
            errors.Add("MerchantKey is required.");
        }

        if (string.IsNullOrEmpty(this.PaymentRequestUri))
        {
            errors.Add("PaymentRequestUri is required.");
        }
        else if (!Uri.IsWellFormedUriString(this.PaymentRequestUri, UriKind.Absolute))
        {
            errors.Add("PaymentRequestUri is not a valid URI.");
        }

        if (string.IsNullOrEmpty(this.PurchasePage))
        {
            errors.Add("PurchasePage is required.");
        }
        else if (!Uri.IsWellFormedUriString(this.PurchasePage, UriKind.Absolute))
        {
            errors.Add("PurchasePage is not a valid URI.");
        }

        return errors;
    }
}