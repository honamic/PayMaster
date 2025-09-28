using FluentAssertions;
using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;
using Xunit;

namespace Honamic.PayMaster.DomianTests.ReceiptRequests;

public class ReceiptRequestGatewayPaymentTests
{
    [Fact]
    public void Create_WithDisabledProvider_ThrowsException()
    {
        // Arrange
        var provider = new PaymentGatewayProfile { Enabled = false };
        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123456,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled,
                MaximumAmount = provider.MaximumAmount,
                MinimumAmount = provider.MinimumAmount
            },
            Amount = 1000,
            Currency = "IRR"
        };

        // Act & Assert
        Assert.Throws<GatewayProviderDisabledException>(() =>
            ReceiptRequestGatewayPayment.Create(parameters));
    }

    [Fact]
    public void Create_WithAmountBelowMinimum_ThrowsException()
    {
        // Arrange
        var provider = new PaymentGatewayProfile
        {
            Enabled = true,
            MinimumAmount = 1000
        };

        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled,
                MaximumAmount = provider.MaximumAmount,
                MinimumAmount = provider.MinimumAmount
            },
            Amount = 500,
            Currency = "IRR"
        };

        // Act & Assert
        Assert.Throws<GatewayMinAmountLimitException>(() =>
            ReceiptRequestGatewayPayment.Create(parameters));
    }

    [Fact]
    public void Create_WithValidParameters_CreatesEntity()
    {
        // Arrange
        var provider = new PaymentGatewayProfile
        {
            Enabled = true,
            Id = 1,
            MinimumAmount = 1000
        };

        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled,
                MaximumAmount = provider.MaximumAmount,
                MinimumAmount = provider.MinimumAmount
            },
            Amount = 5000,
            Currency = "IRR"
        };

        // Act
        var payment = ReceiptRequestGatewayPayment.Create(parameters);

        // Assert
        payment.Should().NotBeNull();
        payment.Id.Should().Be(123);
        payment.Amount.Should().Be(5000);
        payment.Currency.Should().Be("IRR");
        payment.Status.Should().Be(PaymentGatewayStatus.New);
        payment.PaymentGatewayProfileId.Should().Be(1);
    }

    [Fact]
    public void Create_WithAmountAboveMaximum_ThrowsException()
    {
        // Arrange
        var provider = new PaymentGatewayProfile
        {
            Enabled = true,
            MaximumAmount = 10000
        };

        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled,
                MaximumAmount = provider.MaximumAmount,
                MinimumAmount = provider.MinimumAmount
            },
            Amount = 15000,
            Currency = "IRR"
        };

        // Act & Assert
        Assert.Throws<GatewayMaxAmountLimitException>(() =>
            ReceiptRequestGatewayPayment.Create(parameters));
    }

    [Fact]
    public void SetWaitingStatus_SetsCorrectProperties()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        var reference = "REF123";
        var error = "No error";
        var now = DateTimeOffset.Now;

        // Act
        payment.SetWaitingStatus(reference, error, now);

        // Assert
        payment.Status.Should().Be(PaymentGatewayStatus.Waiting);
        payment.CreateReference.Should().Be(reference);
        payment.StatusDescription.Should().Be(error);
        payment.RedirectAt.Should().Be(now);
    }

    [Fact]
    public void SetFailedStatus_SetsCorrectProperties()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        var reason = PaymentGatewayFailedReason.CreateFailed;
        var error = "Creation failed";

        // Act
        payment.SetFailedStatus(reason, error);

        // Assert
        payment.Status.Should().Be(PaymentGatewayStatus.Failed);
        payment.FailedReason.Should().Be(reason);
        payment.StatusDescription.Should().Be(error);
    }

    [Fact]
    public void SetSuccessStatus_SetsCorrectProperties()
    {
        // Arrange
        var payment = CreateValidGatewayPayment(PaymentGatewayStatus.Settlement);
        var supplementaryInfo = new SupplementaryPaymentInformation
        {
            SuccessReference = "SUCCESS123",
            ReferenceRetrievalNumber = "RRN123456789012",
            TrackingNumber = "TR1234",
            Pan = "1234********5678",
            TerminalId = "TERM123",
            MerchantId = "MERCH456"
        };

        // Act
        payment.SuccessVerify(supplementaryInfo);

        // Assert
        payment.Status.Should().Be(PaymentGatewayStatus.Success);
        payment.SuccessReference.Should().Be("SUCCESS123");
        payment.ReferenceRetrievalNumber.Should().Be("RRN123456789012");
        payment.TrackingNumber.Should().Be("TR1234");
        payment.Pan.Should().Be("1234********5678");
        payment.TerminalId.Should().Be("TERM123");
        payment.MerchantId.Should().Be("MERCH456");
    }

    [Fact]
    public void StartCallback_SetsCallbackPropertiesCorrectly()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        var now = DateTimeOffset.Now;
        var callbackData = "{\"data\": \"test data\"}";

        // Set payment to waiting status first (prerequisite for callback)
        payment.SetWaitingStatus("REF123", null, now.AddMinutes(-5));

        // Act
        payment.SetCallback(now, callbackData);

        // Assert
        payment.CallbackAt.Should().Be(now);
        payment.CallbackData.Should().Be(callbackData);
    }

    [Fact]
    public void CanProcessCallback_ReturnsTrueForWaitingStatus()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        payment.SetWaitingStatus("REF123", null, DateTimeOffset.Now);

        // Act
        bool canProcess = payment.CanProcessCallback();

        // Assert
        canProcess.Should().BeTrue();
    }

    [Fact]
    public void CanProcessCallback_ReturnsFalseForNonWaitingStatus()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();

        // Leave in New status

        // Act
        bool canProcess = payment.CanProcessCallback();

        // Assert
        canProcess.Should().BeFalse();
    }

    [Fact]
    public void CanProcessCallback_ReturnsFalseForAlreadyProcessedCallback()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        payment.SetWaitingStatus("REF123", null, DateTimeOffset.Now.AddMinutes(-5));

        // Simulate a callback that's already been processed
        payment.SetCallback(DateTimeOffset.Now, "some data");
        payment.SetFailedStatus(PaymentGatewayFailedReason.CallbackFailed, "Failed callback");

        // Act
        bool canProcess = payment.CanProcessCallback();

        // Assert
        canProcess.Should().BeFalse();
    }

    [Fact]
    public void FailedCallBack_SetsFailurePropertiesCorrectly()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        payment.SetWaitingStatus("REF123", null, DateTimeOffset.Now);
        payment.SetCallback(DateTimeOffset.Now, "callback data");
        var reason = PaymentGatewayFailedReason.Verify;
        var errorMsg = "Verification failed with the gateway";

        // Act
        payment.FailedCallback(reason, errorMsg);

        // Assert
        payment.Status.Should().Be(PaymentGatewayStatus.Failed);
        payment.FailedReason.Should().Be(reason);
        payment.StatusDescription.Should().Be(errorMsg);
    }

    [Fact]
    public void StatusDescription_TruncatesLongErrorMessages()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();
        var veryLongErrorMessage = new string('X', 1000); // Create a long error string

        // Act
        payment.SetFailedStatus(PaymentGatewayFailedReason.Other, veryLongErrorMessage);

        // Assert
        // The actual max length would depend on the implementation
        payment.StatusDescription.Should().NotBeNull();
        payment.StatusDescription!.Length.Should().BeLessThan(veryLongErrorMessage.Length);
    }

    [Fact]
    public void Create_WithInvalidCurrency_ThrowsException()
    {
        // Arrange
        var provider = new PaymentGatewayProfile { Enabled = true };
        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123456,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled,
            },
            Amount = 1000,
            Currency = "" // Empty currency should not be allowed
        };

        // Act & Assert
        Assert.Throws<CurrencyRequiredException>(() =>
            ReceiptRequestGatewayPayment.Create(parameters));
    }

    [Fact]
    public void InvalidStatusTransition_ThrowsException()
    {
        // Arrange
        var payment = CreateValidGatewayPayment();

        // Attempt to transition from New directly to Success without going through Waiting
        // This should fail as it's likely an invalid state transition

        // Act & Assert
        Assert.Throws<InvalidPaymentStatusException>(() =>
            payment.SuccessVerify(new SupplementaryPaymentInformation()));
    }

    // Helper method to create a valid payment for testing status changes
    private ReceiptRequestGatewayPayment CreateValidGatewayPayment(PaymentGatewayStatus? status = null)
    {
        var provider = new PaymentGatewayProfile
        {
            Enabled = true,
            Id = 1
        };

        var parameters = new CreateGatewayPaymentParameters
        {
            Id = 123,
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Id = provider.Id,
                Enabled = provider.Enabled
            },
            Amount = 5000,
            Currency = "IRR"
        };

        var payment = ReceiptRequestGatewayPayment.Create(parameters);

        if (status.HasValue)
        {
            payment.GetType()
                .GetProperty("Status")
                .SetValue(payment, status.Value);
        }

        return payment;
    }
}