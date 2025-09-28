namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles;

public interface IPaymentGatewayProfileRepository
{
    Task<PaymentGatewayProfile?> GetByIdAsync(long id);
    Task<PaymentGatewayProfile?> GetByCodeAsync(string code);
}
