using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Core.PaymentGatewayProviders;

public interface IPaymentGatewayProviderRepository 
    : IRepositoryBase<PaymentGatewayProvider, long>
{

}
