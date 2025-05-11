namespace Honamic.PayMaster.PaymentProviders.ValueObjects;

public static class BaseCurrency
{
    public const string AUD = "AUD";
    public const string BRL = "BRL";
    public const string CAD = "CAD";
    public const string CNY = "CNY";
    public const string CZK = "CZK";
    public const string DKK = "DKK";
    public const string EUR = "EUR";
    public const string HKD = "HKD";
    public const string HUF = "HUF";
    public const string ILS = "ILS";
    public const string JPY = "JPY";
    public const string MYR = "MYR";
    public const string MXN = "MXN";
    public const string TWD = "TWD";
    public const string NZD = "NZD";
    public const string NOK = "NOK";
    public const string PHP = "PHP";
    public const string PLN = "PLN";
    public const string GBP = "GBP";
    public const string SGD = "SGD";
    public const string SEK = "SEK";
    public const string CHF = "CHF";
    public const string THB = "THB";
    public const string USD = "USD";
    public const string IRR = "IRR";

    public static readonly Dictionary<string, string> CurrencySymbols = new()
        {
            { "AUD", "Australian dollar (A$)" },
            { "BRL", "Brazilian real (R$)" },
            { "CAD", "Canadian dollar (C$)" },
            { "CNY", "Chinese Renmenbi (¥)" },
            { "CZK", "Czech koruna (Kč)" },
            { "DKK", "Danish krone (kr)" },
            { "EUR", "Euro (€)" },
            { "HKD", "Hong Kong dollar (HK$)" },
            { "HUF", "Hungarian forint (Ft)" },
            { "ILS", "Israeli new shekel (₪)" },
            { "JPY", "Japanese yen (¥)" },
            { "MYR", "Malaysian ringgit (RM)" },
            { "MXN", "Mexican peso ($)" },
            { "TWD", "New Taiwan dollar (NT$)" },
            { "NZD", "New Zealand dollar (NZ$)" },
            { "NOK", "Norwegian krone (kr)" },
            { "PHP", "Philippine peso (₱)" },
            { "PLN", "Polish złoty (zł)" },
            { "GBP", "Pound sterling (£)" },
            { "SGD", "Singapore dollar (S$)" },
            { "SEK", "Swedish krona (kr)" },
            { "CHF", "Swiss franc (CHF)" },
            { "THB", "Thai baht (฿)" },
            { "USD", "United States dollar ($)" },
            { "IRR", "Iranian Rial (ریال)" }
        };
}