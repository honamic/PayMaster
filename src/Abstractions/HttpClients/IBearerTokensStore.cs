namespace Honamic.PayMaster.HttpClients;

public interface IBearerTokensStore
{
    Task<BearerTokenModel?> GetBearerTokenAsync(string key);
    Task StoreTokenAsync(string key, BearerTokenModel bearerToken);
    Task RemoveTokenAsync(string key);
}