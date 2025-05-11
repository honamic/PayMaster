using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Entities;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class PaymentRequestTest
{
    [Fact]
    public void Create_Should_Return_PaymentRequest_When_Valid_Inputs_Provided()
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var callbackUrl = "https://example.com/callback";
        var uniqueRequestId = 12345;

        var paymentRequest = PaymentRequest.Create(money, callbackUrl, uniqueRequestId);

        paymentRequest.Money.Should().Be(money);
        paymentRequest.CallbackUrl.Should().Be(callbackUrl);
        paymentRequest.UniqueRequestId.Should().Be(uniqueRequestId);
        paymentRequest.PaymentStatus.Should().Be(PaymentStatus.New);
        paymentRequest.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_Money_Is_Null()
    {
        Money? money = null;
        var callbackUrl = "https://example.com/callback";
        var uniqueRequestId = 12345;

        Action action = () => PaymentRequest.Create(money!, callbackUrl, uniqueRequestId);

        action.Should().Throw<ArgumentNullException>().WithMessage("*Money is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Should_Throw_Exception_When_CallbackUrl_Is_Invalid(string callbackUrl)
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var uniqueRequestId = 12345;

        Action action = () => PaymentRequest.Create(money, callbackUrl, uniqueRequestId);

        action.Should().Throw<ArgumentException>().WithMessage("*Callback URL is required*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_Throw_Exception_When_UniqueRequestId_Is_Invalid(long uniqueRequestId)
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var callbackUrl = "https://example.com/callback";

        Action action = () => PaymentRequest.Create(money, callbackUrl, uniqueRequestId);

        action.Should().Throw<ArgumentException>().WithMessage("*Unique Request ID must be a positive number*");
    }

    [Fact]
    public void UpdatePaymentStatus_Should_Update_Status_And_FailedReason_When_Valid()
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        paymentRequest.UpdatePaymentStatus(PaymentStatus.Failed, PaymentFailedReason.Canceled);

        paymentRequest.PaymentStatus.Should().Be(PaymentStatus.Failed);
        paymentRequest.PaymentFailedReason.Should().Be(PaymentFailedReason.Canceled);
        paymentRequest.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePaymentStatus_Should_Throw_Exception_When_Status_Is_Failed_And_Reason_Is_Null()
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        Action action = () => paymentRequest.UpdatePaymentStatus(PaymentStatus.Failed, null);

        action.Should().Throw<InvalidOperationException>().WithMessage("*Failed reason must be provided when status is Failed*");
    }

    [Fact]
    public void SetMobileNumber_Should_Update_MobileNumber_When_Valid()
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        paymentRequest.SetMobileNumber("09123456789");

        paymentRequest.MobileNumber.Should().Be("09123456789");
        paymentRequest.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("123456789012")]
    public void SetMobileNumber_Should_Throw_Exception_When_Invalid(string mobileNumber)
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        Action action = () => paymentRequest.SetMobileNumber(mobileNumber);

        action.Should().Throw<ArgumentException>().WithMessage("*Mobile number must be 11 digits*");
    }

    [Fact]
    public void SetEmail_Should_Update_Email_When_Valid()
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        paymentRequest.SetEmail("test@example.com");

        paymentRequest.Email.Should().Be("test@example.com");
        paymentRequest.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("example.com")]
    public void SetEmail_Should_Throw_Exception_When_Invalid(string email)
    {
        var money = Money.Create(1000, $"{BaseCurrency.USD}");
        var paymentRequest = PaymentRequest.Create(money, "https://example.com/callback", 12345);

        Action action = () => paymentRequest.SetEmail(email);

        action.Should().Throw<ArgumentException>().WithMessage("*Invalid email format*");
    }
}
