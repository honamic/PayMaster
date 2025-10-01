using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;

namespace Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;

public interface IPaymentGatewayProfileQueryModelRepository
{
   public Task<List<GetActivePaymentGatewaysQueryResult>> GetActivePaymentGatewayProfilesAsync(GetActivePaymentGatewaysQuery query, CancellationToken cancellationToken);  
}
