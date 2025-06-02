using Honamic.PayMaster.HttpClients;
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
            await _bearerTokensStore.RemoveTokenAsync(request.RequestUri!.Host);
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

        var tokenModel = await _bearerTokensStore.GetBearerTokenAsync(request.RequestUri!.Host);

        if (tokenModel == null)
        {
            var optionsKey = new HttpRequestOptionsKey<DigipayConfigurations>(Constants.DigiPayRequestOptionsKey);

            if (!request.Options.TryGetValue(optionsKey, out var options))
            {
                throw new Exception($"get {nameof(DigipayConfigurations)} failed.");
            }

            tokenModel = await Login(options, request.RequestUri);
        }

        request.Headers.Authorization =
            tokenModel?.AccessToken is not null ?
            new AuthenticationHeaderValue("Bearer", tokenModel.AccessToken) : null;
    }

    public async Task<BearerTokenModel> Login(DigipayConfigurations configurations, Uri requestUri)
    {
        using var client = _HttpClientFactory.CreateClient(Constants.HttpClientName);
        var loginUrl = new Uri(new Uri(configurations.ApiAddress), Constants.DigiPayAuthPath);
        var authBytes = Encoding.UTF8.GetBytes($"{configurations.ClientId}:{configurations.ClientSecret}");

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);

        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(authBytes)}");

        MultipartFormDataContent content = new MultipartFormDataContent();
        content.Add(new StringContent(configurations.UserName), "username");
        content.Add(new StringContent(configurations.Password), "password");
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
            var bearerTokenModel = tokenData.ToBearerToken();

            await _bearerTokensStore.StoreTokenAsync(request.RequestUri!.Host, bearerTokenModel);

            return bearerTokenModel;
        }

        throw new Exception($"Digipay login failed. result: {tokenResponseString}");
    }
}