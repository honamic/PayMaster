
namespace Honamic.PayMaster.PaymentProviders.Exceptions;

/// <summary>
/// Exception thrown when payment provider configuration is invalid or missing.
/// </summary>
public class PaymentProviderConfigurationException : Exception
{
    public PaymentProviderConfigurationException() : base("Provider configuration is invalid or empty.")
    {
    }

    public PaymentProviderConfigurationException(string message) : base(message)
    {
    }

    public PaymentProviderConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a configuration exception with validation errors.
    /// </summary>
    /// <param name="validationErrors">Collection of validation errors</param>
    public PaymentProviderConfigurationException(IEnumerable<string> validationErrors)
        : base($"Invalid provider configuration: {string.Join(", ", validationErrors)}")
    {
    }
}