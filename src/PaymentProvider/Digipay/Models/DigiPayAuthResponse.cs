using Honamic.PayMaster.PaymentProviders;
using System.Text.Json.Serialization;

namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;

public class DigiPayAuthResponse
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
    public string? JTI { get; set; }

    public BearerTokenModel ToBearerToken()
    {
        return new BearerTokenModel
        {
            AccessToken = AccessToken,
            ExpiresIn = ExpiresIn,
            RefreshToken = RefreshToken,
            TokenType = TokenType,
            Scope = Scope,
            TokenId = JTI,
        };
    }
}