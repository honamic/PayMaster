using Honamic.PayMaster.PaymentProviders.ValueObjects;

namespace Honamic.PayMaster.PaymentProviders.Services;

public static class CardMaskingService
{
    private const int UnmaskedDigits = 4;

    /// <summary>
    /// Mask a card number
    /// </summary>
    /// <param name="cardNumber">The card number to mask</param>
    /// <returns>A masked card number</returns>
    public static string Mask(CardNumber cardNumber)
    {
        var visiblePart = cardNumber.Number.Substring(0 + cardNumber.Number.Length - UnmaskedDigits);
        var maskedPart = new string('*', cardNumber.Number.Length - UnmaskedDigits);

        return maskedPart + visiblePart;
    }
}
