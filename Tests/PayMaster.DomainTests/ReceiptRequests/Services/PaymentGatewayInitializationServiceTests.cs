using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.Options;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace PayMaster.Tests.Domains.ReceiptRequests.Services;

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

    // Helper methods to create test objects
    private ReceiptRequest CreateValidReceiptRequest()
    {

        var receiptRequest = ReceiptRequest.Create(new CreateReceiptRequestParameters
        {
            Amount = 10000, // Set the amount here
            Currency = "IRR",
            Description = "Test payment",
            DefaultGatewayProviderCode = "sandbox",
            DefaultIssuerCode = "default",
            IssuerCode = "",
            SupportedCurrencies = ["IRR", "USD"],
            Issuer = new ReceiptRequestIssuerParameters
            {
                Id = 1, // Use the ID of the created issuer
                Enabled = true
            },
            GatewayProvider= new ReceiptRequestGatewayProviderParameters
            {
                Enabled=true,
                Id=789
            }
        }, idGenerator: _idGenerator.Object);

        return receiptRequest;
    }

    private PaymentGatewayProvider CreateGatewayProvider()
    {
        return new PaymentGatewayProvider
        {
            Id = 789,
            Code = "sandbox",
            Title = "Sandbox Gateway",
            Enabled = true,
            ProviderType = "Honamic.PayMaster.PaymentProvider.Sandbox.SandboxPaymentProvider",
            Configurations = "{\"PayUrl\":\"https://sandbox.com/pay\"}",
            MinimumAmount = 1000,
            MaximumAmount = 50000000
        };
    }
}