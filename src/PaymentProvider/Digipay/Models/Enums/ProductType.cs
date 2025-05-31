namespace Honamic.PayMaster.PaymentProvider.Digipay.Models.Enums;

public enum ProductType
{
    /// <summary>
    /// بادوام
    /// </summary>
    Durable = 1,

    /// <summary>
    /// مصرفی
    /// </summary>
    Consumable = 2,

    /// <summary>
    ///  سرویس یا خدمات 
    /// </summary>
    Service = 3,

    /// <summary>
    /// مصرفی بادوام
    /// </summary>
    DurableConsumable = 4
}
