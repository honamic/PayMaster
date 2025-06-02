using System.Collections.Concurrent;

namespace Honamic.PayMaster.HttpClients;

public class InMemoryBearerTokensStore : IBearerTokensStore
{
    private readonly ConcurrentDictionary<string, BearerTokenModel> Tokens;

    public InMemoryBearerTokensStore()
    {
        Tokens = new ConcurrentDictionary<string, BearerTokenModel>();
    }

    public Task<BearerTokenModel?> GetBearerTokenAsync(string key)
    {
        Tokens.TryGetValue(key, out var token);

        if (token != null && token.ExpiresAt.HasValue && token.ExpiresAt.Value < DateTime.Now)
        {
            Tokens.TryRemove(key, out _);
            return Task.FromResult<BearerTokenModel?>(null);
        }

        return Task.FromResult(token);
    }

    public Task RemoveTokenAsync(string key)
    {
        Tokens.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task StoreTokenAsync(string host, BearerTokenModel bearerToken)
    {
        if (bearerToken == null)
        {
            throw new ArgumentNullException(nameof(bearerToken), "Bearer token cannot be null.");
        }

        bearerToken.SetExpiryFromNow();

        Tokens[host] = bearerToken;
        return Task.CompletedTask;
    }
}