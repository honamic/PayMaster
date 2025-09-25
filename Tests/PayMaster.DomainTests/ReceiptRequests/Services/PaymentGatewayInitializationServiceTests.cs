using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Honamic.PayMaster.Domain.ReceiptRequests.Enums;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Honamic.Framework.Domain;
using Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
using Honamic.PayMaster.Options;

namespace PayMaster.Tests.Domain.ReceiptRequests.Services;

public partial class PaymentGatewayInitializationServiceTests
{
    private readonly Mock<IPaymentGatewayProviderRepository> _repositoryMock;
    private readonly Mock<IPaymentGatewayProviderFactory> _factoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<ILogger<PaymentGatewayInitializationService>> _loggerMock;
    private readonly Mock<IOptions<PayMasterOptions>> _optionsMock;
    private readonly Mock<IPaymentGatewayProvider> _providerMock;
    private readonly Mock<IIdGenerator> _idGenerator;

    private readonly PaymentGatewayInitializationService _service;
    private readonly DateTimeOffset _currentTime = new DateTimeOffset(2023, 5, 12, 10, 30, 0, TimeSpan.Zero);

    public PaymentGatewayInitializationServiceTests()
    {
        _repositoryMock = new Mock<IPaymentGatewayProviderRepository>();
        _factoryMock = new Mock<IPaymentGatewayProviderFactory>();
        _clockMock = new Mock<IClock>();
        _loggerMock = new Mock<ILogger<PaymentGatewayInitializationService>>();
        _optionsMock = new Mock<IOptions<PayMasterOptions>>();
        _providerMock = new Mock<IPaymentGatewayProvider>();
        _idGenerator = new Mock<IIdGenerator>();

        var options = new PayMasterOptions
        {
            CallBackUrl = "https://example.com/callback/{ReceiptRequestId}/{GatewayPaymentId}"
        };

        _optionsMock.Setup(o => o.Value).Returns(options);
        _clockMock.Setup(c => c.NowWithOffset).Returns(_currentTime);
        _idGenerator.Setup(g => g.GetNewId()).Returns(DateTime.Now.Ticks);
        _service = new PaymentGatewayInitializationService(
            _repositoryMock.Object,
            _factoryMock.Object,
            _clockMock.Object,
            _loggerMock.Object,
            _optionsMock.Object);
    }


    [Fact]
    public async Task InitializePaymentAsync_SuccessfulInitialization_ReturnsCreateResult()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();
        var expectedCallbackUrl = $"https://example.com/callback/{receiptRequest.Id}/{gatewayPayment.Id}";

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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(createResult);

        // Act
        var result = await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("REF123", result.GatewayPayment!.CreateReference);
        Assert.Equal("https://gateway.com/pay", result.PayUrl);
        Assert.Equal(PayVerb.Get, result.PayVerb);

        // Verify gateway payment status was updated
        var payment = receiptRequest.GatewayPayments.FirstOrDefault(c => c.CreateReference == createResult.CreateReference);
        Assert.Equal(PaymentGatewayStatus.Waiting, payment?.Status);
        Assert.Equal("REF123", payment?.CreateReference);

        // Verify log entry was created
        Assert.Single(receiptRequest.TryLogs);
        Assert.Equal(ReceiptRequestTryLogType.CreatePaymentProvider, receiptRequest.TryLogs.First().TryType);
        Assert.True(receiptRequest.TryLogs.First().Success);
    }

    [Fact]
    public async Task InitializePaymentAsync_GatewayProviderNotFound_ThrowsException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync((PaymentGatewayProvider)null);

        // Act & Assert
        await Assert.ThrowsAsync<GatewayProviderNotFoundException>(
            () => _service.InitializePaymentAsync(receiptRequest));

    }

    [Fact]
    public async Task InitializePaymentAsync_ProviderReturnsFailure_ReturnsFailedResult()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        var createResult = new CreateResult
        {
            Success = false,
            StatusDescription = "Gateway rejected the request"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(createResult);

        // Act
        var result = await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Gateway rejected the request", result.GatewayPayment!.StatusDescription);

        // Verify log entry was created
        Assert.Single(receiptRequest.TryLogs);
        Assert.Equal(ReceiptRequestTryLogType.CreatePaymentProvider, receiptRequest.TryLogs.First().TryType);
        Assert.False(receiptRequest.TryLogs.First().Success);
    }


    [Fact]
    public async Task InitializePaymentAsync_NullReceiptRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.InitializePaymentAsync(null!));
    }

    [Fact]
    public async Task InitializePaymentAsync_NoGatewayPayments_ThrowsException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        receiptRequest.GatewayPayments.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<PayableGatewayPaymentNotFoundException>(
            () => _service.InitializePaymentAsync(receiptRequest));
    }

    [Fact]
    public async Task InitializePaymentAsync_FactoryThrowsException_ReturnsFailedResult()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);
      
        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
                .Throws<PaymentProviderNotFoundException>();
        // Act & Assert
        await Assert.ThrowsAsync<PaymentProviderNotFoundException>(
            () => _service.InitializePaymentAsync(receiptRequest));
    }

    [Fact]
    public async Task InitializePaymentAsync_ProviderThrowsNetworkException_ReturnsFailedResult()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayProvider = ReceiptRequestsHelper.CreateGatewayProvider();

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.CreateAsync(It.IsAny<CreateRequest>()))
            .ReturnsAsync(() =>
            {
                var result = new CreateResult();
                result.LogData.SetException(new Exception("Network error"));
                result.StatusDescription = "Network error";
                return result;
            });

        // Act
        var result = await _service.InitializePaymentAsync(receiptRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Network error", result.GatewayPayment?.StatusDescription);
        Assert.Single(receiptRequest.TryLogs);
        Assert.False(receiptRequest.TryLogs.First().Success);
    }

    [Fact]
    public async Task InitializePaymentAsync_GatewayPaymentNotInNewStatus_ThrowsException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateValidReceiptRequest(_idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        gatewayPayment.SetWaitingStatus("REF123", "Already initialized", DateTimeOffset.Now); // Change status from New

        // Act & Assert
        await Assert.ThrowsAsync<PayableGatewayPaymentNotFoundException>(
            () => _service.InitializePaymentAsync(receiptRequest));
    }

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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
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

        _factoryMock.Setup(f => f.Create(gatewayProvider.ProviderType, gatewayProvider.JsonConfigurations))
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