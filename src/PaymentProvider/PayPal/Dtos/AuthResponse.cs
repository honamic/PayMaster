using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.PayPal.Dtos;

public class AuthResponse
{
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}