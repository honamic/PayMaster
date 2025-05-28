using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;

namespace Honamic.PayMaster.Domains.ReceiptRequests.Services;
public interface ICreateReceiptRequestDomainService
{
    Task<ReceiptRequest> CreateAsync(CreateReceiptRequestParameters createParams);
}
