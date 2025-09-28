using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;

namespace Honamic.PayMaster.Domain.ReceiptRequests.Services;
public interface ICreateReceiptRequestDomainService
{
    Task<ReceiptRequest> CreateAsync(CreateReceiptRequestParameters createParams);
}
