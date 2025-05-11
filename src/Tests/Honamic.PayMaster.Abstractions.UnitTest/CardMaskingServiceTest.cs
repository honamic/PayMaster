using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.Services;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class CardMaskingServiceTest
{
    [Theory]
    [InlineData("5892120488774391", "************4391")]
    [InlineData("6334 1389 7772 5595", "***************5595")]
    public void Mask_Should_Return_Masked_CardNumber(string unmaskedCardNumber, string expectedMaskedCardNumber)
    {
        var cardNumber = CardNumber.Create(unmaskedCardNumber);

        var maskedCardNumber = CardMaskingService.Mask(cardNumber);

        maskedCardNumber.Should().Be(expectedMaskedCardNumber);
    }
}
