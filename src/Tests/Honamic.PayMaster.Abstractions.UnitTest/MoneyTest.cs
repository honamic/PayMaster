using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class MoneyTest
{
    [Theory]
    [InlineData($"{BaseCurrency.IRR}", 5000000)]
    [InlineData($"{BaseCurrency.USD}", 50)]
    [InlineData($"{BaseCurrency.EUR}", 10.50)]
    public void Create_Should_Return_Money_When_Amount_And_Currency_Valid(string currency, decimal amount)
    {
        var money = Money.Create(amount, currency);

        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Create_Should_Return_Error_When_Currency_Is_Null()
    {
        Action action = () => Money.Create(100, null);

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("ABCD")]
    public void Create_Should_Return_Error_When_Currency_Is_Not_3_Characters(string currency)
    {
        Action action = () => Money.Create(100, currency);

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_Should_Return_Error_When_Amount_Is_Lower_Or_Equal_To_Zero(decimal amount)
    {
        Action action = () => Money.Create(amount, $"{BaseCurrency.USD}");

        action.Should().Throw<InvalidAmountException>();
    }
}
