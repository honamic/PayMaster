using System.Text.Json.Serialization;

namespace Honamic.PayMaster.HttpClients;

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

    public void SetExpireAtNow()
    {
        ExpiresAt = DateTime.Now.AddSeconds(ExpiresIn);
    }
}