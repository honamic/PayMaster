using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.Options;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptIssuers;
using Honamic.PayMaster.Domains.ReceiptIssuers.Parameters;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace PayMaster.Tests.Domains.ReceiptRequests.Services;

public partial class CallbackGatewayPaymentDomainServiceTests
{
    private readonly Mock<IPaymentGatewayProviderRepository> _repositoryMock;
    private readonly Mock<IPaymentGatewayProviderFactory> _factoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<ILogger<CallbackGatewayPaymentDomainService>> _loggerMock;
    private readonly Mock<IOptions<PayMasterOptions>> _optionsMock;
    private readonly Mock<IPaymentGatewayProvider> _providerMock;
    private readonly Mock<IIdGenerator> _idGenerator;

    private CallbackGatewayPaymentDomainService _service;
    private readonly DateTimeOffset _currentTime = new DateTimeOffset(2023, 5, 12, 10, 30, 0, TimeSpan.Zero);

    public CallbackGatewayPaymentDomainServiceTests()
    {
        _repositoryMock = new Mock<IPaymentGatewayProviderRepository>();
        _factoryMock = new Mock<IPaymentGatewayProviderFactory>();
        _clockMock = new Mock<IClock>();
        _loggerMock = new Mock<ILogger<CallbackGatewayPaymentDomainService>>();
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
    }

    [Fact]
    public async Task ProcessCallbackAsync_ValidRequest_ReturnsSuccessfulResult()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Doing, _idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        gatewayPayment.SetWaitingStatus("ref123", "Redirected to payment gateway", _currentTime.AddMinutes(-5));

        var receiptIssuer = ReceiptIssuer.Create(new ReceiptIssuerParameters
        {
            Id = 123,
            Description = "Test Issuer Description",
            Code = "TestIssuer",
            Title = "Test Issuer",
            Enabled = true,
            CallbackUrl = "/payment/{ReceiptRequestId}/{Status}"
        });

        var gatewayProvider = new PaymentGatewayProvider
        {
            Id = 100,
            Code = "test",
            Title = "Test Provider",
            ProviderType = "TestProvider",
            Configurations = "{}",
            Enabled = true
        };

        var extractResult = new ExtractCallBackDataResult
        {
            Success = true,
            UniqueRequestId = gatewayPayment.Id,
            CreateReference = gatewayPayment.CreateReference,
            CallBack = new { }
        };

        var verifyResult = new VerifyResult
        {
            Success = true,
            SupplementaryPaymentInformation = new SupplementaryPaymentInformation
            {
                Pan = "123456******7890",
                SuccessReference = "ref123456",
                ReferenceRetrievalNumber = "123456789012",
                TrackingNumber = "123456",
            }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(gatewayProvider);

        _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_providerMock.Object);

        _providerMock.Setup(p => p.ExtractCallBackData(It.IsAny<string>()))
            .Returns(extractResult);

        _providerMock.Setup(p => p.GetCallbackValidityDuration())
            .Returns(TimeSpan.FromMinutes(30));

        _providerMock.Setup(p => p.VerifyAsync(It.IsAny<VerifyRequest>()))
            .ReturnsAsync(verifyResult);

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(receiptRequest);

        var receiptIssuerRepositoryMock = new Mock<IReceiptIssuerRepository>();
        receiptIssuerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(receiptIssuer);

        _service = new CallbackGatewayPaymentDomainService(
            receiptRequestRepositoryMock.Object,
            _repositoryMock.Object,
            receiptIssuerRepositoryMock.Object,
            _factoryMock.Object,
            Mock.Of<IUnitOfWork>(),
            _clockMock.Object);

        // Act
        var result = await _service.ProcessCallbackAsync(
            receiptRequest.Id,
            gatewayPayment.Id,
            "{\"data\": \"sample callback data\"}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(receiptRequest, result.ReceiptRequest);
        Assert.Equal(gatewayPayment, result.GatewayPayment);
        Assert.Contains(receiptRequest.Id.ToString(), result.IssuerCallbackUrl);
        Assert.Equal(ReceiptRequestStatus.Done, receiptRequest.Status);
    }

    [Fact]
    public async Task ProcessCallbackAsync_ReceiptRequestNotFound_ThrowsInvalidPaymentException()
    {
        // Arrange
        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync((ReceiptRequest)null!);

        _service = new CallbackGatewayPaymentDomainService(
            receiptRequestRepositoryMock.Object,
            _repositoryMock.Object,
            Mock.Of<IReceiptIssuerRepository>(),
            _factoryMock.Object,
            Mock.Of<IUnitOfWork>(),
            _clockMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPaymentException>(() =>
            _service.ProcessCallbackAsync(1, 1, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_InvalidReceiptRequestStatus_ThrowsPaymentStatusNotValidForProcessingException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Done, _idGenerator.Object);

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(receiptRequest);

        _service = new CallbackGatewayPaymentDomainService(
            receiptRequestRepositoryMock.Object,
            _repositoryMock.Object,
            Mock.Of<IReceiptIssuerRepository>(),
            _factoryMock.Object,
            Mock.Of<IUnitOfWork>(),
            _clockMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<PaymentStatusNotValidForProcessingException>(() =>
            _service.ProcessCallbackAsync(receiptRequest.Id, 1, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_GatewayPaymentNotFound_ThrowsInvalidPaymentException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Doing, _idGenerator.Object);

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(receiptRequest);

        _service = new CallbackGatewayPaymentDomainService(
            receiptRequestRepositoryMock.Object,
            _repositoryMock.Object,
            Mock.Of<IReceiptIssuerRepository>(),
            _factoryMock.Object,
            Mock.Of<IUnitOfWork>(),
            _clockMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPaymentException>(() =>
            _service.ProcessCallbackAsync(receiptRequest.Id, 999, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_InvalidGatewayPaymentStatus_ThrowsPaymentStatusNotValidForProcessingException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Doing, _idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        gatewayPayment.SetFailedStatus(PaymentGatewayFailedReason.Canceled, "User canceled the payment");

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(receiptRequest);

        _service = new CallbackGatewayPaymentDomainService(
            receiptRequestRepositoryMock.Object,
            _repositoryMock.Object,
            Mock.Of<IReceiptIssuerRepository>(),
            _factoryMock.Object,
            Mock.Of<IUnitOfWork>(),
            _clockMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<PaymentStatusNotValidForProcessingException>(() =>
            _service.ProcessCallbackAsync(receiptRequest.Id, gatewayPayment.Id, "{}"));
    }
}