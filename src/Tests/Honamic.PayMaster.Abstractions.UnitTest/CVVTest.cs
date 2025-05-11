using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class CVVTest
{
    [Theory]
    [InlineData("665")]
    [InlineData("000")]
    [InlineData("1234")]
    public void Create_Should_Return_CVV_When_Code_Is_Valid(string code)
    {
        var cvv = CVV.Create(code);

        cvv.Code.Should().Be(code);
    }

    [Fact]
    public void Create_Should_Return_Error_When_Code_Is_Null()
    {
        Action action = () => CVV.Create(null);

        action.Should().Throw<ArgumentNullException>().WithMessage("*code*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    public void Create_Should_Return_Error_When_Code_Is_Not_Three_Or_Four_Characters(string code)
    {
        Action action = () => CVV.Create(code);

        action.Should().Throw<InvalidCVVException>().WithMessage($"{code} is not a valid CVV code");
    }
}
