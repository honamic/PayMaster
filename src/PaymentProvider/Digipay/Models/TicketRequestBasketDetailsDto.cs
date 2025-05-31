using Honamic.PayMaster.PaymentProvider.Digipay.Models.Enums;

namespace Honamic.PayMaster.PaymentProvider.Digipay.Models;

public class TicketRequestBasketDetailsDto
{
    public TicketRequestBasketDetailsDto()
    {
        Items = [];
    }
    /// <summary>
    /// آیدی یونیک به ازای هر سبد خرید	
    /// </summary>
    public required string BasketId { get; set; }

    /// <summary>
    /// لیستی از موارد درون سبد	
    /// </summary>
    public TicketRequestBasketDetailItemDto[] Items { get; set; }
}

public class TicketRequestBasketDetailItemDto
{
    /// <summary>
    ///  کد محصول
    /// </summary>
    public required string ProductCode { get; set; }

    /// <summary>
    /// تعداد کالا
    /// </summary>
    public int Count { get; set; }


    /// <summary>
    /// نوع کالا 
    /// </summary>
    public ProductType ProductType { get; set; }


    /// <summary>
    /// دسته‌بندی کالا
    /// </summary>
    public DigipayCategory CategoryId { get; set; }


    /// <summary>
    /// شناسه فروشنده
    /// </summary>
    public string? SellerId { get; set; }

    /// <summary>
    /// شناسه تامین کننده
    /// </summary>
    public string? SupplierId { get; set; }
    
    /// <summary>
    ///  شناسه برند
    /// </summary>
    public string? Brand { get; set; }
}