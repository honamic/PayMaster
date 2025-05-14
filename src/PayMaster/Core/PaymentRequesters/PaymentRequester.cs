using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Core.PaymentRequesters;

public class PaymentRequester: AggregateRoot<long>
{
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