using Honamic.Framework.Persistence.EntityFramework;

namespace Honamic.PayMaster.Domains.PaymentGatewayProviders;

public interface IPaymentGatewayProviderRepository 
    : IRepositoryBase<PaymentGatewayProvider, long>
{

}
