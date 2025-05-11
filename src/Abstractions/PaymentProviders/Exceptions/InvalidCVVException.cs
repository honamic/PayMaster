namespace Honamic.PayMaster.PaymentProviders.Exceptions;

public class InvalidCVVException : Exception
{
    public InvalidCVVException()
    {
    }

    public InvalidCVVException(string message)
        : base(message)
    {
    }

    public InvalidCVVException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
