using Honamic.PayMaster.PaymentProvider.PayPal.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.PayPal.HttpClients;

public class HttpInterceptorService : DelegatingHandler
{
    private readonly IBearerTokensStore _bearerTokensStore;
    private readonly IHttpClientFactory _HttpClientFactory;

    public HttpInterceptorService(
        IBearerTokensStore bearerTokensStore,
        IHttpClientFactory httpClientFactory)
    {
        _bearerTokensStore = bearerTokensStore ?? throw new ArgumentNullException(nameof(bearerTokensStore));
        _HttpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        await AddAccessTokenToAllRequestsAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _bearerTokensStore.RemoveToken();
        }

        return response;
    }

    private async Task AddAccessTokenToAllRequestsAsync(HttpRequestMessage request)
    {
        if (request.RequestUri?.AbsolutePath == Constants.PayPalAuthPath)
        {
            return;
        }

        var optionsKey = new HttpRequestOptionsKey<PayPalConfigurations>(Constants.PayPalRequestOptionsKey);

        if (!request.Options.TryGetValue(optionsKey, out var options))
        {
            throw new Exception($"get PayPalConfigurations failed.");
        }

        var token = await _bearerTokensStore.GetBearerTokenAsync();

        if (token == null && options.AutoLogin)
        {
            token = await Login(options);
        }

        request.Headers.Authorization =
            token is not null ? new AuthenticationHeaderValue("Bearer", token) : null;
    }

    public async Task<string> Login(PayPalConfigurations payPalConfigurations)
    {
        using var client = _HttpClientFactory.CreateClient(Constants.HttpClientName);
        var authBytes = Encoding.UTF8.GetBytes($"{payPalConfigurations.ClientId}:{payPalConfigurations.Secret}");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var tokenResponse = await client.PostAsync($"{payPalConfigurations.ApiAddress}/v1/oauth2/token",
            new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded"));

        var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new Exception($"paypal login failed. {tokenResponseString}");
        }

        var tokenData = JsonSerializer.Deserialize<AuthResponse>(tokenResponseString);

        if (!string.IsNullOrEmpty(tokenData?.AccessToken))
        {
            var expire = DateTime.Now.AddDays(tokenData.ExpiresIn);
            await _bearerTokensStore.StoreToken(tokenData.AccessToken, expire);
            return tokenData.AccessToken;
        }

        throw new Exception($"paypal login failed. result: {tokenResponseString}");
    }
}