using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles.Parameters;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptIssuers.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PayMaster.Tests.Domain.ReceiptRequests.Services;

public partial class CallbackGatewayPaymentDomainServiceTests
{
    private readonly Mock<IPaymentGatewayProfileRepository> _repositoryMock;
    private readonly Mock<IPaymentGatewayProviderFactory> _factoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<ILogger<CallbackGatewayPaymentDomainService>> _loggerMock;
    private readonly Mock<IPaymentGatewayProvider> _providerMock;
    private readonly Mock<IIdGenerator> _idGenerator;
    private readonly string CallBackUrl;
    private CallbackGatewayPaymentDomainService _service;
    private readonly DateTimeOffset _currentTime = new DateTimeOffset(2023, 5, 12, 10, 30, 0, TimeSpan.Zero);

    public CallbackGatewayPaymentDomainServiceTests()
    {
        _repositoryMock = new Mock<IPaymentGatewayProfileRepository>();
        _factoryMock = new Mock<IPaymentGatewayProviderFactory>();
        _clockMock = new Mock<IClock>();
        _loggerMock = new Mock<ILogger<CallbackGatewayPaymentDomainService>>();
        _providerMock = new Mock<IPaymentGatewayProvider>();
        _idGenerator = new Mock<IIdGenerator>();
        CallBackUrl = "https://example.com/callback/{ReceiptRequestId}/{GatewayPaymentId}";
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



        var createParameters = new CreatePaymentGatewayProfileParameters
        {
            Id = 100,
            Code = "test",
            Title = "Test Provider",
            ProviderType = "TestProvider",
            JsonConfigurations = "{}",
            Enabled = true
        };

        var gatewayProvider = PaymentGatewayProfile.Create(createParameters);

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

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), CancellationToken.None))
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
        receiptRequestRepositoryMock.Setup(r => r.GetByGatewayPaymentIDAsync(It.IsAny<long>(), CancellationToken.None))
            .ReturnsAsync(receiptRequest);

        var receiptIssuerRepositoryMock = new Mock<IReceiptIssuerRepository>();
        receiptIssuerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), CancellationToken.None))
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
        receiptRequestRepositoryMock.Setup(r => r.GetByGatewayPaymentIDAsync(It.IsAny<long>(), CancellationToken.None))
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
            _service.ProcessCallbackAsync(1, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_InvalidReceiptRequestStatus_ThrowsPaymentStatusNotValidForProcessingException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Done, _idGenerator.Object);

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByGatewayPaymentIDAsync(It.IsAny<long>(), CancellationToken.None))
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
            _service.ProcessCallbackAsync(receiptRequest.Id, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_GatewayPaymentNotFound_ThrowsInvalidPaymentException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Doing, _idGenerator.Object);

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByGatewayPaymentIDAsync(It.IsAny<long>(), CancellationToken.None))
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
            _service.ProcessCallbackAsync(999, "{}"));
    }

    [Fact]
    public async Task ProcessCallbackAsync_InvalidGatewayPaymentStatus_ThrowsPaymentStatusNotValidForProcessingException()
    {
        // Arrange
        var receiptRequest = ReceiptRequestsHelper.CreateSampleReceiptRequestForStatus(ReceiptRequestStatus.Doing, _idGenerator.Object);
        var gatewayPayment = receiptRequest.GatewayPayments.First();
        gatewayPayment.SetFailedStatus(PaymentGatewayFailedReason.Canceled, "User canceled the payment");

        var receiptRequestRepositoryMock = new Mock<IReceiptRequestRepository>();
        receiptRequestRepositoryMock.Setup(r => r.GetByGatewayPaymentIDAsync(It.IsAny<long>(), CancellationToken.None))
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
            _service.ProcessCallbackAsync(gatewayPayment.Id, "{}"));
    }
}