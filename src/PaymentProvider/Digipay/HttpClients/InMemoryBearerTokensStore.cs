namespace Honamic.PayMaster.PaymentProvider.DigiPay.HttpClients;

public class InMemoryBearerTokensStore : IBearerTokensStore
{
    private static string? Token;
    private static DateTime? Expire;

    public Task<string?> GetBearerTokenAsync()
    {
        if (Expire.HasValue && DateTime.Now.AddSeconds(15) < Expire.Value)
            return Task.FromResult(Token);

        return Task.FromResult((string?)null);
    }

    public Task RemoveToken()
    {
        Token = null;
        Expire = null;
        return Task.CompletedTask;
    }

    public Task StoreToken(string token, DateTime expire)
    {
        Token = token;
        Expire = expire;
        return Task.CompletedTask;
    }
}