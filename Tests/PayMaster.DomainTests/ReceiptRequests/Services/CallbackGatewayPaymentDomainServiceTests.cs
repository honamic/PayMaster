using Honamic.Framework.Domain;
using Honamic.PayMaster.Application.Options;
using Honamic.PayMaster.Domains.PaymentGatewayProviders;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;
using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

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

    private readonly PaymentGatewayInitializationService _service;
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

}