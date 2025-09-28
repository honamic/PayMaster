using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptIssuers.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
using Honamic.PayMaster.Options;
using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace PayMaster.Tests.Domain.ReceiptRequests.Services;

public partial class CreateReceiptRequestDomainServiceTests
{
    private readonly Mock<IPaymentGatewayProfileRepository> _repositoryMock;
    private readonly Mock<IPaymentGatewayProviderFactory> _factoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<ILogger<CreateReceiptRequestDomainService>> _loggerMock;
    private readonly Mock<IOptions<PayMasterOptions>> _optionsMock;
    private readonly Mock<IPaymentGatewayProvider> _providerMock;
    private readonly Mock<IIdGenerator> _idGenerator;

    private CreateReceiptRequestDomainService _service;
    private readonly DateTimeOffset _currentTime = new DateTimeOffset(2023, 5, 12, 10, 30, 0, TimeSpan.Zero);

    public CreateReceiptRequestDomainServiceTests()
    {
        _repositoryMock = new Mock<IPaymentGatewayProfileRepository>();
        _factoryMock = new Mock<IPaymentGatewayProviderFactory>();
        _clockMock = new Mock<IClock>();
        _loggerMock = new Mock<ILogger<CreateReceiptRequestDomainService>>();
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



    private readonly Mock<IReceiptRequestRepository> _receiptRequestRepositoryMock = new();
    private readonly Mock<IReceiptIssuerRepository> _receiptIssuerRepositoryMock = new();
    private readonly Mock<IPaymentGatewayProfileRepository> _paymentGatewayProfileRepositoryMock = new();

    private CreateReceiptRequestDomainService CreateService()
    {
        return new CreateReceiptRequestDomainService(
            _idGenerator.Object,
            _receiptRequestRepositoryMock.Object,
            _receiptIssuerRepositoryMock.Object,
            _paymentGatewayProfileRepositoryMock.Object
        );
    }

    private static CreateReceiptRequestParameters GetValidParams() => new()
    {
        Amount = 1000,
        Currency = "IRR",
        IssuerCode = "issuer1",
        DefaultIssuerCode = "defaultIssuer",
        GatewayProviderId = 1,
        GatewayProviderCode = "gateway1",
        DefaultGatewayProviderCode = "defaultGateway",
        SupportedCurrencies = new[] { "IRR" }
    };

    [Fact]
    public async Task CreateAsync_Should_Create_ReceiptRequest_Successfully()
    {
        // Arrange
        var parameters = GetValidParams();

        var issuer = ReceiptIssuer.Create(new ReceiptIssuerParameters
        {
            Id = 10,
            Code = "issuer1",
            Enabled = true,
            Title = "Test",
            CallbackUrl = "cb",
            Description = ""
        });

        var gateway = ReceiptRequestsHelper.CreateGatewayProfile();

        _receiptIssuerRepositoryMock.Setup(x => x.GetByCodeAsync("issuer1", CancellationToken.None)).ReturnsAsync(issuer);
        _paymentGatewayProfileRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(gateway);
        _receiptRequestRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<ReceiptRequest>(), CancellationToken.None)).Returns(Task.CompletedTask);

        _idGenerator.Setup(x => x.GetNewId()).Returns(123);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(parameters.Amount, result.Amount);
        Assert.Equal(parameters.Currency, result.Currency);
        Assert.Equal(issuer.Id, result.IssuerId);
        Assert.Equal(gateway.Id, result.GatewayPayments.First().PaymentGatewayProfileId);
        _receiptRequestRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<ReceiptRequest>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_NoIssuerSpecifiedException_When_IssuerCode_And_DefaultIssuerCode_Are_Null()
    {
        // Arrange
        var parameters =  new CreateReceiptRequestParameters()
        {
            IssuerCode = null,
            DefaultIssuerCode = null,
            
            Amount = 1000,
            Currency = "IRR",
            GatewayProviderId = 1,
            GatewayProviderCode = "gateway1",
            DefaultGatewayProviderCode = "defaultGateway",
            SupportedCurrencies = new[] { "IRR" }
        };

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<NoIssuerSpecifiedException>(() => service.CreateAsync(parameters));
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_IssuerCodeNotFoundException_When_Issuer_Not_Found()
    {
        // Arrange
        var parameters = GetValidParams();
        _receiptIssuerRepositoryMock.Setup(x => x.GetByCodeAsync("issuer1", CancellationToken.None)).ReturnsAsync((ReceiptIssuer?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<IssuerCodeNotFoundException>(() => service.CreateAsync(parameters));
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_NoDefaultGatewayProviderException_When_All_GatewayProvider_Identifiers_Are_Null()
    {
        // Arrange
        var parameters = new CreateReceiptRequestParameters()
        {
            GatewayProviderId = null,
            GatewayProviderCode = null,
            DefaultGatewayProviderCode = null,
            
            Amount = 1000,
            Currency = "IRR",
            IssuerCode = "sales",
            DefaultIssuerCode = "default",
            SupportedCurrencies = ["IRR"]
        };


        var issuer = ReceiptIssuer.Create(new ReceiptIssuerParameters
        {
            Id = 10,
            Code = "sales",
            Enabled = true,
            Title = "Test",
            CallbackUrl = "cb",
            Description = ""
        });

        _receiptIssuerRepositoryMock.Setup(x => x.GetByCodeAsync("sales", CancellationToken.None)).ReturnsAsync(issuer);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<NoDefaultGatewayProviderException>(() => service.CreateAsync(parameters));
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_SpecifiedGatewayProviderNotFoundException_When_GatewayProvider_Not_Found()
    {
        // Arrange
        var parameters = GetValidParams();
        var issuer = ReceiptIssuer.Create(new ReceiptIssuerParameters
        {
            Id = 10,
            Code = "issuer1",
            Enabled = true,
            Title = "Test",
            CallbackUrl = "cb",
            Description = ""
        });
        _receiptIssuerRepositoryMock.Setup(x => x.GetByCodeAsync("issuer1", CancellationToken.None)).ReturnsAsync(issuer);
        _paymentGatewayProfileRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((PaymentGatewayProfile?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<SpecifiedGatewayProviderNotFoundException>(() => service.CreateAsync(parameters));
    }

}