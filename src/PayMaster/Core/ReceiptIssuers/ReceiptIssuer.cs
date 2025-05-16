using Honamic.Framework.Domain;

namespace Honamic.PayMaster.Core.ReceiptIssuers;

public class ReceiptIssuer: AggregateRoot<long>
{
    public ReceiptIssuer()
    {
        
    }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public bool Enabled { get; set; }

    public bool ShowPaymentResultPage { get; set; }

    public string? CallbackUrl { get; set; }
    
    public string? WebHookUrl { get; set; }

    public string? Description { get; private set; }
}