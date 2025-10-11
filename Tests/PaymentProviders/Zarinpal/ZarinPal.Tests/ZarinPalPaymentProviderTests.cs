using Moq;
using System.Net;
using System.Text.Json;
using ZarinPal.Tests.Helper;
using Microsoft.Extensions.Logging;
using Honamic.PayMaster.PaymentProvider.ZarinPal;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Models;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;

namespace ZarinPal.Tests;

public partial class ZarinPalPaymentProviderTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<ZarinPalPaymentProvider>> _loggerMock;

    public ZarinPalPaymentProviderTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<ZarinPalPaymentProvider>>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenApiResponseIsValid()
    {
        // Arrange
        var config = new ZarinPalConfigurations
        {
            ApiAddress = "https://sandbox.zarinpal.com/",
            PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
            MerchantId = "test-merchant"
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(new ZarinPalResult<PaymentRequestResponse>
        {
            data = new PaymentRequestResponse { Code = 100, Authority = "AUTH123" }
        }), HttpStatusCode.OK);

        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);

        provider.SetConfiguration(config);

        var request = new CreateRequest
        {
            Amount = 10000,
            Currency = "IRR",
            CallbackUrl = "https://callback.com",
            UniqueRequestId = 1,
            GatewayNote = "test",
            MobileNumber = "09120000000",
            Email = "test@test.com"
        };

        // Act
        var result = await provider.CreateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("AUTH123", result.PayUrl);
        Assert.Equal(PayVerb.Get, result.PayVerb);
    }

    [Fact]
    public void ExtractCallBackData_ShouldReturnSuccess_WhenStatusIsOK()
    {
        // Arrange
        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        var callbackJson = JsonSerializer.Serialize(new CallBackDataModel
        {
            Status = "OK",
            Authority = "AUTH456"
        });

        // Act
        var result = provider.ExtractCallBackData(callbackJson);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("AUTH456", result.CreateReference);
    }

    [Fact]
    public void ExtractCallBackData_ShouldReturnCanceled_WhenStatusIsNOK()
    {
        // Arrange
        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        var callbackJson = JsonSerializer.Serialize(new CallBackDataModel
        {
            Status = "NOK",
            Authority = "AUTH789"
        });

        // Act
        var result = provider.ExtractCallBackData(callbackJson);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.Canceled, result.PaymentFailedReason);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnSuccess_WhenApiResponseIsValid()
    {
        // Arrange
        var config = new ZarinPalConfigurations
        {
            ApiAddress = "https://sandbox.zarinpal.com/",
            PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
            MerchantId = "test-merchant"
        };

        var callbackData = new CallBackDataModel
        {
            Authority = "AUTH123",
            Status = "OK"
        };

        var successResponse = new ZarinPalResult<PaymentVerificationResponse>
        {
            data = new PaymentVerificationResponse
            {
                code = 100,
                ref_id = 987654321,
                card_pan = "1234-****-****-5678"
            }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(successResponse), HttpStatusCode.OK);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        provider.SetConfiguration(config);

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = 1,
                Amount = 10000,
                CreateReference = "AUTH123"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("987654321", result.SupplementaryPaymentInformation.SuccessReference);
        Assert.Equal("1234-****-****-5678", result.SupplementaryPaymentInformation.Pan);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenApiResponseCodeIsInvalid()
    {
        // Arrange
        var config = new ZarinPalConfigurations
        {
            ApiAddress = "https://sandbox.zarinpal.com/",
            PayUrl = "https://sandbox.zarinpal.com/pg/StartPay/",
            MerchantId = "test-merchant"
        };

        var callbackData = new CallBackDataModel
        {
            Authority = "AUTH123",
            Status = "OK"
        };

        var failedResponse = new ZarinPalResult<PaymentVerificationResponse>
        {
            data = new PaymentVerificationResponse
            {
                code = -51 // failed payment code
            }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(failedResponse), HttpStatusCode.OK);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        provider.SetConfiguration(config);

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = 1,
                Amount = 10000,
                CreateReference = "AUTH123"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.Verify, result.PaymentFailedReason);
        Assert.Contains("پرداخت ناموفق", result.StatusDescription);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenInternalVerificationFails()
    {
        // Arrange
        var config = new ZarinPalConfigurations
        {
            ApiAddress = "https://sandbox.zarinpal.com/",
            MerchantId = "test-merchant"
        };

        config.SetDefaultConfiguration();

        var callbackData = new CallBackDataModel
        {
            Authority = "WRONG_AUTH", // This doesn't match the request's CreateReference
            Status = "OK"
        };

        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        provider.SetConfiguration(config);

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = 1,
                Amount = 10000,
                CreateReference = "AUTH123" // Different from callback's Authority
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.InternalVerify, result.PaymentFailedReason);
        Assert.Contains("مغایرت", result.StatusDescription);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var config = new ZarinPalConfigurations
        {
            ApiAddress = "https://sandbox.zarinpal.com/",
            MerchantId = "test-merchant"
        };

        config.SetDefaultConfiguration();

        var callbackData = new CallBackDataModel
        {
            Authority = "AUTH123",
            Status = "OK"
        };

        // Setup HttpClient to throw exception
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Network error"));

        var provider = new ZarinPalPaymentProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        provider.SetConfiguration(config);

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = 1,
                Amount = 10000,
                CreateReference = "AUTH123"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Network error", result.StatusDescription);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
