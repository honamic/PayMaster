namespace Honamic.PayMaster.PaymentProviders.ValueObjects;

public class Payer
{
    protected Payer(string payerId, string? name = null, string? phone = null, string? email = null)
    {
        PayerId = payerId;
        Name = name;
        Phone = phone;
        Email = email;
    }

    public string PayerId { get; private set; }
    public string? Name { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }


    public static Payer Create(string payerId, string? name = null, string? phone = null, string? email = null)
    {
        return new Payer(payerId, name, phone, email);
    }
}
