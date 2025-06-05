using System.Runtime.Serialization;

namespace Honamic.PayMaster.PaymentProviders.Exceptions;

[Serializable]
public class PaymentProviderNotFoundException : Exception
{
    public PaymentProviderNotFoundException() { }

    public PaymentProviderNotFoundException(string message) : base(message) { }

    public PaymentProviderNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    protected PaymentProviderNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}