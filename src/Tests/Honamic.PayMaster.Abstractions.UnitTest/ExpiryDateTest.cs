using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Exceptions;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class ExpiryDateTest
{
    [Theory]
    [InlineData(2, 27)]
    [InlineData(5, 26)]
    [InlineData(12, 26)]
    public void Create_Should_Return_ExpiryDate_When_Month_Year_Is_Valid(int month, int year)
    {
        var expiryDate = ExpiryDate.Create(year, month);

        expiryDate.Year.Should().Be(year);
        expiryDate.Month.Should().Be(month);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(13, 23)]
    public void Create_Should_Return_Error_When_Month_Is_Invalid(int month, int year)
    {
        Action action = () => ExpiryDate.Create(year, month);

        action.Should().Throw<InvalidExpiryDateException>().WithMessage($"{month}/{year} is not a valid expiry date");
    }

    [Theory]
    [InlineData(10, 19)]
    [InlineData(9, 1)]
    public void Create_Should_Return_Error_When_Year_Is_In_Past(int month, int year)
    {
        Action action = () => ExpiryDate.Create(year, month);

        action.Should().Throw<InvalidExpiryDateException>().WithMessage("Invalid expiry date, card expired");
    }
}
