using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;

namespace Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;

public interface IPaymentGatewayProfileQueryModelRepository
{
   public Task<List<GetActivePaymentGatewaysQueryResult>> GetActivePaymentGatewayProfilesAsync(GetActivePaymentGatewaysQuery query, CancellationToken cancellationToken);  
   public Task<PagedQueryResult<GetAllPaymentGatewaysQueryResult>> GetAll(GetAllPaymentGatewaysQuery query, CancellationToken cancellationToken);  
   public Task<GetPaymentGatewayQueryResult?> Get(GetPaymentGatewayQuery query, CancellationToken cancellationToken);  
}
