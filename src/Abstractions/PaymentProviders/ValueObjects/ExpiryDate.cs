using Honamic.PayMaster.PaymentProviders.Exceptions;

namespace Honamic.PayMaster.PaymentProviders.ValueObjects;

public class ExpiryDate
{
    public int Year { get; private set; }
    public int Month { get; private set; }

    private ExpiryDate(int year, int month)
    {
        Year = year;
        Month = month;
    }

    public static ExpiryDate Create(int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new InvalidExpiryDateException($"{month}/{year} is not a valid expiry date");
        }

        //TODO: it works only on forieng countries cards, should be globalized by culture
        if (DateTime.Now.Year > year + 2000 || DateTime.Now.Year == year + 2000 && DateTime.Now.Month > month)
        {
            throw new InvalidExpiryDateException("Invalid expiry date, card expired");
        }

        return new ExpiryDate(year, month);
    }
}

