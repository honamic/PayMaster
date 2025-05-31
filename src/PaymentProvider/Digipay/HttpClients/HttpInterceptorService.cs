using Honamic.PayMaster.PaymentProvider.Digipay.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.Digipay.HttpClients;

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
            await _bearerTokensStore.RemoveTokenAsync();
        }

        return response;
    }

    private async Task AddAccessTokenToAllRequestsAsync(HttpRequestMessage request)
    {
        if (request.RequestUri?.AbsolutePath.TrimEnd('/')
            .EndsWith(Constants.DigiPayAuthPath.TrimEnd('/')) == true)
        {
            return;
        }

        var token = await _bearerTokensStore.GetBearerTokenAsync();

        if (token == null)
        {
            var optionsKey = new HttpRequestOptionsKey<DigipayConfigurations>(Constants.DigiPayRequestOptionsKey);

            if (!request.Options.TryGetValue(optionsKey, out var options))
            {
                throw new Exception($"get {nameof(DigipayConfigurations)} failed.");
            }

            token = await Login(options);
        }

        request.Headers.Authorization =
            token is not null ? new AuthenticationHeaderValue("Bearer", token) : null;
    }

    public async Task<string> Login(DigipayConfigurations configurations)
    {
        using var client = _HttpClientFactory.CreateClient(Constants.HttpClientName);
        var loginUrl = new Uri(new Uri(configurations.ApiAddress), Constants.DigiPayAuthPath);
        var authBytes = Encoding.UTF8.GetBytes($"{configurations.ClientId}:{configurations.ClientSecret}");

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);

        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(authBytes)}");

        MultipartFormDataContent content = new MultipartFormDataContent();
        content.Add(new StringContent(configurations.UserName),"username" );
        content.Add(new StringContent(configurations.Password),"password");
        content.Add(new StringContent("password"), "grant_type");
        request.Content = content;

        var tokenResponse = await client.SendAsync(request);

        var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Digipay login failed. {tokenResponseString}");
        }

        var tokenData = JsonSerializer.Deserialize<DigiPayAuthResponse>(tokenResponseString);

        if (!string.IsNullOrEmpty(tokenData?.AccessToken))
        {
            var expire = DateTime.Now.AddDays(tokenData.ExpiresIn);
            await _bearerTokensStore.StoreTokenAsync(tokenData.AccessToken, expire);
            return tokenData.AccessToken;
        }

        throw new Exception($"Digipay login failed. result: {tokenResponseString}");
    }
}