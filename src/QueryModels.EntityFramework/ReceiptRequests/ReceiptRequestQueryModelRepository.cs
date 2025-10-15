using Honamic.Framework.EntityFramework.QueryModels;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.QueryModels.EntityFramework.ReceiptRequests;

internal class ReceiptRequestQueryModelRepository : IReceiptRequestQueryModelRepository
{
    private readonly DbContext _context;

    public ReceiptRequestQueryModelRepository([FromKeyedServices(QueryConstants.QueryDbContextKey)] DbContext context)
    {
        _context = context;
    }

    public Task<PagedQueryResult<GetAllReceiptRequestsQueryResult>> GetAll(GetAllReceiptRequestsQuery query, CancellationToken cancellationToken)
    {
        return _context.Set<ReceiptRequestQueryModel>()
                    .Select(c => new GetAllReceiptRequestsQueryResult
                    {
                        Id = c.Id,
                        Status = c.Status,
                        Amount = c.Amount,
                        Currency = c.Currency,
                        Mobile = c.Mobile,
                        NationalityCode = c.NationalityCode,
                        Email = c.Email,
                        Description = c.Description,
                        IsLegal = c.IsLegal,
                        IssuerId = c.IssuerId,
                        IssuerReference = c.IssuerReference,
                        PartyReference = c.PartyReference,
                        IssuerTitle = c.Issuer.Title,
                        PartyId = c.PartyId,
                    })
                    .ToFilteredPagedListAsync(query, cancellationToken);
    }

    public Task<GetPublicReceiptRequestQueryResult?> GetPublicAsync(GetPublicReceiptRequestQuery query, CancellationToken cancellationToken)
    {
        return _context.Set<ReceiptRequestQueryModel>()
              .Where(c => c.Id == query.Id)
              .Select(c => new GetPublicReceiptRequestQueryResult
              {
                  Id = c.Id,
                  Status = c.Status,
                  Amount = c.Amount,
                  Currency = c.Currency,
                  Mobile = c.Mobile,
                  NationalityCode = c.NationalityCode,
                  Email = c.Email,
                  GatewayPayments = c.GatewayPayments.Select(g => new GetPublicReceiptRequestGatewayPaymentQueryResult
                  {
                      Id = g.Id,
                      Amount = g.Amount,
                      Currency = g.Currency,
                      Status = g.Status,
                      PaymentGatewayTitle = g.PaymentGatewayProfile.Title,
                      RedirectAt = g.RedirectAt,
                      SuccessReference = g.SuccessReference,
                      ReferenceRetrievalNumber = g.ReferenceRetrievalNumber,
                      TrackingNumber = g.TrackingNumber,
                      Pan = g.Pan
                  }).ToList()
              }).FirstOrDefaultAsync(cancellationToken);
    }
}