namespace Honamic.PayMaster.PaymentProvider.Digipay.HttpClients;

public interface IBearerTokensStore
{
    Task<string?> GetBearerTokenAsync();
    Task StoreTokenAsync(string token, DateTime expire);
    Task RemoveTokenAsync();
}
