using Honamic.Framework.Persistence.EntityFramework;
using Honamic.PayMaster.Domain.ReceiptRequests;
using System.Linq.Expressions;

namespace Honamic.PayMaster.Persistence.ReceiptRequests;

internal class ReceiptRequestRepository
    : RepositoryBase<ReceiptRequest, long>
    , IReceiptRequestRepository
{
    public ReceiptRequestRepository(PaymasterDbContext context) : base(context)
    {

    }

    public Task<ReceiptRequest?> GetByGatewayPaymentCreateReferenceAsync(string? createReference, long gatewayProviderId, CancellationToken cancellationToken = default)
    {
        return GetAsync(c => c.GatewayPayments.Any(c => c.CreateReference == createReference
        && c.GatewayProviderId == gatewayProviderId),cancellationToken);
    }

    public Task<ReceiptRequest?> GetByGatewayPaymentIDAsync(long gatewayPaymentId, CancellationToken cancellationToken = default)
    {
        return GetAsync(receipt => receipt.GatewayPayments.Any(pay => pay.Id == gatewayPaymentId), cancellationToken);
    }

    public Task<ReceiptRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return GetAsync(c => c.Id == id, cancellationToken);
    }

    protected override IList<Expression<Func<ReceiptRequest, object?>>> GetIncludes()
    {
        return new List<Expression<Func<ReceiptRequest, object?>>>
        {
            c=>c.GatewayPayments
        };
    }
}
