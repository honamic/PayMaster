using Honamic.PayMaster.PaymentProviders.Exceptions;

namespace Honamic.PayMaster.PaymentProviders.ValueObjects;

public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount <= 0)
            throw new InvalidAmountException();

        if (string.IsNullOrEmpty(currency) || !BaseCurrency.CurrencySymbols.ContainsKey(currency.ToUpper()))
            throw new InvalidCurrencyException();

        return new Money(amount, currency.ToUpper());
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add amounts with different currencies.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract amounts with different currencies.");

        if (Amount < other.Amount)
            throw new InvalidOperationException("Resulting amount cannot be negative.");

        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";

    public override bool Equals(object? obj) =>
        obj is Money money && Amount == money.Amount && Currency == money.Currency;

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}