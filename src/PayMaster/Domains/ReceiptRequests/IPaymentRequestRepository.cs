namespace Honamic.PayMaster.Domains.ReceiptRequests;

public interface IReceiptRequestRepository
{
    Task<ReceiptRequest?> GetByIdAsync(long id);
    Task InsertAsync(ReceiptRequest entity);
    Task<ReceiptRequest?> GetByGatewayPaymentCreateReferenceAsync(string? createReference, long gatewayProviderId);
    Task<ReceiptRequest?> GetByGatewayPaymentIDAsync(long gatewayPaymentId);
}