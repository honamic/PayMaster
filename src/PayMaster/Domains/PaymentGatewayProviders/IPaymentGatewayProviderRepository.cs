namespace Honamic.PayMaster.Domains.PaymentGatewayProviders;

public interface IPaymentGatewayProviderRepository
{
    Task<PaymentGatewayProvider?> GetByIdAsync(long id);
    Task<PaymentGatewayProvider?> GetByCodeAsync(string code);
}
