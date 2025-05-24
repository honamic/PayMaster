using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Domains.ReceiptRequests;

public interface IReceiptRequestRepository
    : IRepositoryBase<ReceiptRequest, long>
{
    Task<ReceiptRequest?> GetByGatewayPaymentCreateReferenceAsync(string? createReference, long gatewayProviderId);
    Task<ReceiptRequest?> GetByGatewayPaymentIDAsync(long gatewayPaymentId);
}