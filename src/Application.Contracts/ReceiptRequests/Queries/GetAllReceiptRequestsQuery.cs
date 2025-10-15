﻿using Honamic.Framework.Application.Authorizes;
using Honamic.Framework.Application.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.ReceiptRequests;

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

public class GetAllReceiptRequestsQueryResult
{
    public long Id { get; set; }

    public ReceiptRequestStatus Status { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;

    public long IssuerId { get; set; }

    public string IssuerTitle { get; set; } = default!;

    public string? Description { get; set; }

    public string? Mobile { get; set; }
    
    public string? NationalityCode { get; set; }
    
    public string? Email { get; set; }
    
    public bool? IsLegal { get; set; }
    
    public string? IssuerReference { get; set; }
    
    public string? PartyReference { get; set; }

    public long? PartyId { get; set; }
}