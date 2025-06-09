using System.Net;
using System.Text;

namespace PayPal.Tests.Helper;

// Helper classes for testing HTTP requests
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        });
    }
}

public class MockHttpMessageHandlerSequence : HttpMessageHandler
{
    private readonly Queue<(string response, HttpStatusCode statusCode)> _responses;

    public MockHttpMessageHandlerSequence(IEnumerable<(string response, HttpStatusCode statusCode)> responses)
    {
        _responses = new Queue<(string, HttpStatusCode)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_responses.TryDequeue(out var response))
        {
            throw new InvalidOperationException("No more responses available");
        }

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = response.statusCode,
            Content = new StringContent(response.response, Encoding.UTF8, "application/json")
        });
    }
}

