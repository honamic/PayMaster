namespace Honamic.PayMaster.Domains.ReceiptRequests;

public interface IReceiptRequestRepository
{
    Task<ReceiptRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task InsertAsync(ReceiptRequest entity, CancellationToken cancellationToken = default);
    Task<ReceiptRequest?> GetByGatewayPaymentCreateReferenceAsync(string? createReference, long gatewayProviderId, CancellationToken cancellationToken = default);
    Task<ReceiptRequest?> GetByGatewayPaymentIDAsync(long gatewayPaymentId, CancellationToken cancellationToken = default);
}