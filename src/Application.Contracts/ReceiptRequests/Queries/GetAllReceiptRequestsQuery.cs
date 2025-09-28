using Honamic.Framework.Applications.Authorizes;
using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;

namespace Honamic.PayMaster.Application.ReceiptRequests.Queries;

[DynamicPermission(
    DisplayName = "لیست درخواست پرداخت ها",
    Group = "ReceiptRequests",
    Module = PayMasterConstants.ModuleName,
    Name = null,
    Description = "")]
 
public class GetAllReceiptRequestsQuery: PagedQueryFilter, IQuery<Result<PagedQueryResult<GetAllReceiptRequestsQueryResult>>>
{
    public GetAllReceiptRequestsQuery()
    {
        OrderBy = "Id desc";
    }
}
