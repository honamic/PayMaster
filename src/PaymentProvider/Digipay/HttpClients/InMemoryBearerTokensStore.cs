namespace Honamic.PayMaster.PaymentProvider.Digipay.HttpClients;

public class InMemoryBearerTokensStore : IBearerTokensStore
{
    private static string? Token;
    private static DateTime? Expire;
    private static readonly object LockObject = new();

    public Task<string?> GetBearerTokenAsync()
    {
        lock (LockObject)
        {
            if (Expire.HasValue && DateTime.Now.AddSeconds(15) < Expire.Value)
                return Task.FromResult(Token);
        }
        return Task.FromResult((string?)null);
    }

    public Task RemoveTokenAsync()
    {
        lock (LockObject)
        {
            Token = null;
            Expire = null;
        }
        return Task.CompletedTask;
    }

    public Task StoreTokenAsync(string token, DateTime expire)
    {
        lock (LockObject)
        {
            Token = token;
            Expire = expire;
        }
        return Task.CompletedTask;
    }
}