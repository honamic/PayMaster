using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;
public interface ICreateReceiptRequestDomainService
{
    Task<ReceiptRequest> CreateAsync(CreateReceiptRequestParameters createParams);
    Task<ReceiptRequest> RepayAsync(long receiptRequestId, string? gatewayProviderCode,
        long? gatewayProviderId, string? defaultGatewayProviderCode, CancellationToken cancellationToken);
}
