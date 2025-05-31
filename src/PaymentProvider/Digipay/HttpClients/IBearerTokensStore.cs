namespace Honamic.PayMaster.PaymentProvider.Digipay.HttpClients;

public interface IBearerTokensStore
{
    Task<string?> GetBearerTokenAsync();
    Task StoreToken(string token, DateTime expire);
    Task RemoveToken();
}
