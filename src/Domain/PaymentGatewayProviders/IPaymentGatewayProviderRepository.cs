namespace Honamic.PayMaster.Domain.PaymentGatewayProviders;

public interface IPaymentGatewayProviderRepository
{
    Task<PaymentGatewayProvider?> GetByIdAsync(long id);
    Task<PaymentGatewayProvider?> GetByCodeAsync(string code);
}
