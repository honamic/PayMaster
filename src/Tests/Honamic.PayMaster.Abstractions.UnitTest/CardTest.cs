using FluentAssertions;
using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.Abstractions.UnitTest;

public class CardTest
{
    [Fact]
    public void Create_Should_Return_Card_When_Valid()
    {
        var cardNumber = CardNumber.Create("5892120488774391");
        var cvv = CVV.Create("665");
        var expiryDate = ExpiryDate.Create(29, 10);
        var ownerName = "Hamed Seifi";

        var card = Card.Create(cardNumber, cvv, expiryDate, ownerName);

        card.Number.Should().Be(cardNumber);
        card.Cvv.Should().Be(cvv);
        card.ExpiryDate.Should().Be(expiryDate);
        card.OwnerName.Should().Be(ownerName);
    }
}
