using Honamic.Framework.Domain;
using Honamic.PayMaster.Domains.ReceiptIssuers.Parameters;

namespace Honamic.PayMaster.Domains.ReceiptIssuers;

public class ReceiptIssuer: AggregateRoot<long>
{
    public ReceiptIssuer()
    {
        
    }

    public string Code { get; private set; } = default!;

    public string Title { get; private set; } = default!;

    public bool Enabled { get; private set; }

    public string CallbackUrl { get; private set; } = default!;
    
    public string? Description { get; private set; }

    public static ReceiptIssuer Create(ReceiptIssuerParameters create)
    {
        var newReceiptIssuer = new ReceiptIssuer
        {
            Id = create.Id,
            CallbackUrl = create.CallbackUrl,
            Code = create.Code,
            Title = create.Title,
            Enabled = create.Enabled,
            Description = create.Description,
        };

        return newReceiptIssuer;
    }
}