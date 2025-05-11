using System.Text.RegularExpressions;
using Honamic.PayMaster.PaymentProviders.Exceptions;

namespace Honamic.PayMaster.PaymentProviders.ValueObjects;

public class CardNumber
{
    public string Number { get; private set; }

    private CardNumber(string number)
    {
        Number = number;
    }

    public static CardNumber Create(string number)
    {
        if (number is null)
        {
            throw new ArgumentNullException(nameof(number));
        }
        if (!ValidateNumber(number))
        {
            throw new InvalidCardNumberException($"{number} is not a valid card number");
        }

        return new CardNumber(number);
    }

    /// <summary>
    /// Checks if the card number is in a valid format
    /// </summary>
    /// <param name="number">The card number</param>
    /// <returns>valid/not valid</returns>
    private static bool ValidateNumber(string number)
    {
        return Regex.Replace(number, @"\s+", "").Length == 16;
    }
}
