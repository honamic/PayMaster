using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries; 

namespace Honamic.PayMaster.Application.ReceiptRequests.Queries;
 
public class GetPublicReceiptRequestQuery : IQuery<Result<GetPublicReceiptRequestQueryResult?>>
{
    public long Id { get; set; }
}