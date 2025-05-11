using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class CardNumberTest
{
    [Theory]
    [InlineData("5892120488774391")]
    [InlineData("5465984640800225")]
    [InlineData("6011 7854 6419 8118")]
    public void Create_Should_Return_CardNumber_When_Number_Is_Valid(string number)
    {
        var cardnumber = CardNumber.Create(number);

        cardnumber.Number.Should().Be(number);
    }

    [Fact]
    public void Create_Should_Return_Error_When_Number_Is_Null()
    {
        Action action = () => CardNumber.Create(null);

        action.Should().Throw<ArgumentNullException>().WithMessage("*number*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("37921204887743")]
    public void Create_Should_Return_Error_When_Number_Is_Invalid(string number)
    {
        Action action = () => CardNumber.Create(number);

        action.Should().Throw<InvalidCardNumberException>().WithMessage($"{number} is not a valid card number");
    }
}
