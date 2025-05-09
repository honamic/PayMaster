namespace Honamic.PayMaster.Core.PaymentRequesters;

public class PaymentRequester
{
    public long Id { get; set; }

    /// <summary>
    /// sample: 100 , Ordering
    /// </summary>
    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public bool ShowPaymentResultPage { get; set; }

    /// <summary>
    /// Yourlink/{PaymentRequestId}
    /// </summary>
    public string? CallbackUrl { get; set; }
    
    public string? WebHookUrl { get; set; }

    public string? Description { get; private set; }
}