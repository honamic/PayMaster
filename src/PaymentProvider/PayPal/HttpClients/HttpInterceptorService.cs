using Honamic.PayMaster.Extensions;
using Honamic.PayMaster.PaymentProvider.PayPal.Models;
using Honamic.PayMaster.PaymentProviders;
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
            await _bearerTokensStore.RemoveTokenAsync(request.RequestUri.GetOrigin());
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

        var token = await _bearerTokensStore.GetBearerTokenAsync(request.RequestUri.GetOrigin());

        if (token == null)
        {
            token = await Login(options, request.RequestUri!);
        }

        request.Headers.Authorization =
            token is not null
            ? new AuthenticationHeaderValue("Bearer", token.AccessToken)
            : null;
    }

    public async Task<BearerTokenModel> Login(PayPalConfigurations payPalConfigurations, Uri requestUri)
    {
        using var client = _HttpClientFactory.CreateClient(Constants.HttpClientName);
        var authBytes = Encoding.UTF8.GetBytes($"{payPalConfigurations.ClientId}:{payPalConfigurations.Secret}");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var loginUrl = new Uri(new Uri(payPalConfigurations.ApiAddress), Constants.PayPalAuthPath);

        var tokenResponse = await client.PostAsync(loginUrl,
            new StringContent("grant_type=client_credentials", Encoding.UTF8,
            "application/x-www-form-urlencoded"));

        var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new Exception($"paypal login failed. {tokenResponseString}");
        }

        var tokenData = JsonSerializer.Deserialize<PayPalAuthResponse>(tokenResponseString);

        if (!string.IsNullOrEmpty(tokenData?.AccessToken))
        {
            var bearerToken = tokenData.ToBearerToken();

            await _bearerTokensStore.StoreTokenAsync(requestUri.GetOrigin(), bearerToken);

            return bearerToken;
        }

        throw new Exception($"paypal login failed. result: {tokenResponseString}");
    }
}