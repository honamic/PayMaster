
namespace Honamic.PayMaster.Domain.PaymentGatewayProfiles;

public interface IPaymentGatewayProfileRepository
{
    Task<PaymentGatewayProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PaymentGatewayProfile?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task InsertAsync(PaymentGatewayProfile paymentGatewayProfile, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(string code,long? currentId, CancellationToken cancellationToken = default);
}
