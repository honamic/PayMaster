using Moq;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Honamic.PayMaster.PaymentProvider.PayPal;
using Honamic.PayMaster.PaymentProvider.PayPal.Models;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Enums;
using Honamic.PayMaster.PaymentProvider.PayPal.Models.Shared;
using Honamic.PayMaster.PaymentProviders.Models;
using PayPal.Tests.Helper;
using Honamic.PayMaster.ReceiptRequests;

namespace PayPal.Tests;

public partial class PayPalPaymentProviderTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<PayPalPaymentProvider>> _loggerMock;

    public PayPalPaymentProviderTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<PayPalPaymentProvider>>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenApiResponseIsValid()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        var responseLinks = new[]
        {
            new PaypalOrderLink
            {
                Href = "https://www.sandbox.paypal.com/checkoutnow?token=123456789",
                Rel = "payer-action",
                Method = "GET"
            }
        };

        var successResponse = new PaypalCreateOrderResponse
        {
            Id = "ORDER123456789",
            Links = responseLinks
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(successResponse), HttpStatusCode.Created);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var request = new CreateRequest
        {
            Amount = 100.50m,
            Currency = "USD",
            CallbackUrl = "https://example.com/callback",
            UniqueRequestId = DateTime.Now.Ticks,
            Email = "customer@example.com"
        };

        // Act
        var result = await provider.CreateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("ORDER123456789", result.CreateReference);
        Assert.Equal("https://www.sandbox.paypal.com/checkoutnow?token=123456789", result.PayUrl);
        Assert.Equal(PayVerb.Get, result.PayVerb);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenApiResponseIsNotSuccessful()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        var handler = new MockHttpMessageHandler("Error response", HttpStatusCode.BadRequest);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var request = new CreateRequest
        {
            Amount = 100.50m,
            Currency = "USD",
            CallbackUrl = "https://example.com/callback",
            UniqueRequestId = DateTime.Now.Ticks
        };

        // Act
        var result = await provider.CreateAsync(request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleException_WhenErrorOccurs()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Network error"));

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var request = new CreateRequest
        {
            Amount = 100.50m,
            Currency = "USD",
            CallbackUrl = "https://example.com/callback",
            UniqueRequestId = DateTime.Now.Ticks
        };

        // Act
        var result = await provider.CreateAsync(request);

        // Assert
        Assert.False(result.Success);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void ExtractCallBackData_ShouldReturnSuccess_WhenTokenExists()
    {
        // Arrange
        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        var callbackData = new PayPalCallBackDataModel
        {
            Token = "TOKEN123456789"
        };
        var callbackJson = JsonSerializer.Serialize(callbackData);

        // Act
        var result = provider.ExtractCallBackData(callbackJson);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("TOKEN123456789", result.CreateReference);
    }

    [Fact]
    public void ExtractCallBackData_ShouldReturnFailure_WhenTokenIsMissing()
    {
        // Arrange
        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        var callbackData = new PayPalCallBackDataModel
        {
            Token = null
        };
        var callbackJson = JsonSerializer.Serialize(callbackData);

        // Act
        var result = provider.ExtractCallBackData(callbackJson);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("token value is empty", result.Error);
    }

    [Fact]
    public void ExtractCallBackData_ShouldHandleException_WhenJsonIsInvalid()
    {
        // Arrange
        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        var invalidJson = "{invalid-json}";

        // Act
        var result = provider.ExtractCallBackData(invalidJson);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnSuccess_WhenApiResponsesAreValid()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        // Setup GetOrder response
        var getOrderResponse = new PayPalOrder
        {
            Id = "ORDER123456789",
            Status = PayPalOrderStatus.Approved,
            Intent = PayPalCheckoutPaymentIntent.Capture,
            PurchaseUnits = new[]
            {
                new PayPalPurchaseUnit
                {
                    Amount = new PayPalMoney
                    {
                        Value = "100.50",
                        CurrencyCode = "USD"
                    }
                }
            }
        };

        // Setup Capture response
        var captureResponse = new PayPalOrder
        {
            Id = "ORDER123456789",
            Status = PayPalOrderStatus.Completed,
            Intent = PayPalCheckoutPaymentIntent.Capture,
            PurchaseUnits = new[]
            {
                new PayPalPurchaseUnit
                {
                    Payments = new PayPalPaymentCollection
                    {
                        Captures = new[]
                        {
                            new PayPalOrdersCaptureModel
                            {
                                Id = "CAPTURE987654321"
                            }
                        }
                    }
                }
            }
        };

        // Create mock handlers for sequential requests
        var mockSequence = new MockHttpMessageHandlerSequence(new[]
        {
            (JsonSerializer.Serialize(getOrderResponse), HttpStatusCode.OK),
            (JsonSerializer.Serialize(captureResponse), HttpStatusCode.Created)
        });

        var client = new HttpClient(mockSequence);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var callbackData = new PayPalCallBackDataModel
        {
            Token = "ORDER123456789"
        };

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = DateTime.Now.Ticks,
                Amount = 100.50m,
                CreateReference = "ORDER123456789"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("CAPTURE987654321", result.SupplementaryPaymentInformation.SuccessReference);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenOrderStatusNotApproved()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        // Setup GetOrder response with non-approved status
        var getOrderResponse = new PayPalOrder
        {
            Id = "ORDER123456789",
            Status = PayPalOrderStatus.Created, // Not approved
            Intent = PayPalCheckoutPaymentIntent.Capture,
            PurchaseUnits = new[]
            {
                new PayPalPurchaseUnit
                {
                    Amount = new PayPalMoney
                    {
                        Value = "100.50",
                        CurrencyCode = "USD"
                    }
                }
            }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(getOrderResponse), HttpStatusCode.OK);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var callbackData = new PayPalCallBackDataModel
        {
            Token = "ORDER123456789"
        };

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = DateTime.Now.Ticks,
                Amount = 100.50m,
                CreateReference = "ORDER123456789"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.Verify, result.PaymentFailedReason);
        Assert.Contains("Status not Valid", result.StatusDescription);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenAmountDoesNotMatch()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        // Setup GetOrder response with different amount
        var getOrderResponse = new PayPalOrder
        {
            Id = "ORDER123456789",
            Status = PayPalOrderStatus.Approved,
            Intent = PayPalCheckoutPaymentIntent.Capture,
            PurchaseUnits = new[]
            {
                new PayPalPurchaseUnit
                {
                    Amount = new PayPalMoney
                    {
                        Value = "200.50", // Different amount than requested
                        CurrencyCode = "USD"
                    }
                }
            }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(getOrderResponse), HttpStatusCode.OK);
        var client = new HttpClient(handler);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var callbackData = new PayPalCallBackDataModel
        {
            Token = "ORDER123456789"
        };

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = DateTime.Now.Ticks,
                Amount = 100.50m, // Different from response amount
                CreateReference = "ORDER123456789"
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.Verify, result.PaymentFailedReason);
        Assert.Contains("Amount not Valid", result.StatusDescription);
    }

    [Fact]
    public async Task VerifyAsync_ShouldReturnFailure_WhenInternalVerificationFails()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var callbackData = new PayPalCallBackDataModel
        {
            Token = "DIFFERENT_TOKEN" // Different from reference
        };

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = DateTime.Now.Ticks,
                Amount = 100.50m,
                CreateReference = "ORDER123456789" // Different from token
            },
            CallBackData = callbackData
        };

        // Act
        var result = await provider.VerifyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PaymentGatewayFailedReason.InternalVerify, result.PaymentFailedReason);
    }

    [Fact]
    public async Task VerifyAsync_ShouldHandleException_WhenErrorOccurs()
    {
        // Arrange
        var config = new PayPalConfigurations
        {
            ClientId = "test-client-id",
            Secret = "test-secret",
            ApiAddress = "https://api-m.sandbox.paypal.com"
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Network error"));

        var provider = new PayPalPaymentProvider(_loggerMock.Object, _httpClientFactoryMock.Object);
        provider.SetConfiguration(config);

        var callbackData = new PayPalCallBackDataModel
        {
            Token = "ORDER123456789"
        };

        var request = new VerifyRequest
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                UniqueRequestId = DateTime.Now.Ticks,
                Amount = 100.50m,
                CreateReference = "ORDER123456789"
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