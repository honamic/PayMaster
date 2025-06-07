using Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders.Models;
using Moq;
using Xunit;

namespace PayMaster.Tests.Domains.ReceiptRequests.Services;

public partial class PaymentGatewayInitializationServiceTests
{
    [Fact]
    public async Task InitializePaymentAsync_PaymentCurrency_RetainedInPaymentRequest()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();
        var expectedCurrency = gatewayPayment.Currency;

        var createResult = new CreateResult
        {
            Success = true,
            CreateReference = "REF123",
            PayParams = new Dictionary<string, string> { { "token", "TOKEN123" } },
            PayUrl = "https://gateway.com/pay",
            PayVerb = PayVerb.Get
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(createResult);

        // Act
        var result = await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.Equal(expectedCurrency, result.GatewayPayment!.Currency);
        
        // Verify currency was correctly passed to provider
        _providerMock.Verify(p => p.CreateAsync(It.Is<CreateRequest>(
            req => req.Currency == expectedCurrency)), Times.Once);
    }

    [Fact]
    public async Task InitializePaymentAsync_PartiallyImplementedGateway_HandlesGracefully()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        var createResult = new CreateResult
        {
            Success = true,
            CreateReference = "REF123",
            // No PayParams provided
            PayUrl = null, // PayUrl is null
            PayVerb = PayVerb.Post // But PayVerb is provided
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(createResult);

        // Act
        var result = await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.PayUrl);
        Assert.Equal(PayVerb.Post, result.PayVerb);
        Assert.NotNull(result.PayParams); // Should initialize an empty dictionary, not null
    }

    [Fact]
    public async Task InitializePaymentAsync_LogsCreatedCorrectly()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        var createResult = new CreateResult
        {
            Success = true,
            CreateReference = "REF123"
        };

        createResult.LogData.SetMessage("Test log message");
        
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(createResult);

        // Act
        await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.Single(receiptRequest.TryLogs);
        var log = receiptRequest.TryLogs.First();
        Assert.Equal("Test log message", log.Data.Message);
        Assert.Equal(gatewayPayment.Id, log.ReceiptRequestGatewayPaymentId);
        Assert.Equal(receiptRequest.Id, log.ReceiptRequestId);
    }
    
    [Fact]
    public async Task InitializePaymentAsync_CorrectCallbackUrlGenerated()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();
        
        var expectedCallbackUrl = $"https://example.com/callback/{receiptRequest.Id}/{gatewayPayment.Id}";

        CreateRequest? capturedRequest = null;

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .Callback<CreateRequest>(req => capturedRequest = req)
            .ReturnsAsync(new CreateResult { Success = true });

        // Act
        await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(expectedCallbackUrl, capturedRequest!.CallbackUrl);
    }

    [Fact]
    public async Task InitializePaymentAsync_StatusUpdated_WhenSuccessful()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        var initialStatus = receiptRequest.Status;
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.Configurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(new CreateResult { Success = true });

        // Act
        await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.NotEqual(initialStatus, receiptRequest.Status);
        Assert.Equal(ReceiptRequestStatus.Doing, receiptRequest.Status);
    }
}