using Honamic.Framework.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.ReceiptIssuers;

public class ReceiptIssuerQueryModel : AggregateQueryBase<long>
{
    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public bool Enabled { get; set; }

    public string CallbackUrl { get; set; } = default!;

    public string? Description { get; set; }

    public ICollection<ReceiptRequestQueryModel> ReceiptRequests = new HashSet<ReceiptRequestQueryModel>();
}