using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProviders;

public interface IBearerTokensStore
{
    Task<BearerTokenModel?> GetBearerTokenAsync(string key);
    Task StoreTokenAsync(string key, BearerTokenModel bearerToken);
    Task RemoveTokenAsync(string key);
}


public class BearerTokenModel
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("jti")]
    public string? TokenId { get; set; }

    public DateTime? ExpiresAt { get; private set; }

    public void SetExpiryFromNow()
    {
        ExpiresAt = DateTime.Now.AddSeconds(ExpiresIn);
    }
}