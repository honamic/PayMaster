
using Honamic.PayMaster.PaymentProvider.DigiPay.Models.Enums;

namespace Honamic.PayMaster.PaymentProvider.DigiPay.Models;

public class TicketRequestSplitDetailsDto
{
    public required DigipaySplitType Type { get; set; }

    /// <summary>
    /// نام کاربری دریافت‌کننده مبلغ تسهیم
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// مبلغ تسهیم
    /// </summary>
    public required long Amount { get; set; }

    public List<TicketRequestSplitDetailPolicyDto>? Policies { get; set; } // لیست سیاست‌ها (در صورت تسهیم بیمه)

    public TicketRequestSplitDetailPolicyHolderDto? PolicyHolder { get; set; } // اطلاعات دارنده بیمه (در صورت تسهیم بیمه)
}

public class TicketRequestSplitDetailPolicyDto
{
    public string? Id { get; set; }

    public long Price { get; set; }
    
    public long PriceWithDiscount { get; set; }

    /// <summary>
    /// گونه
    /// </summary>
    public required string VariantId { get; set; }

    public required DigipayCategory Category { get; set; }
   
    public required string Brand { get; set; } 
 
    public required string Model { get; set; }
    
    public string? SerialNo { get; set; }
}

public class TicketRequestSplitDetailPolicyHolderDto
{
    public string? NationalCode { get; set; } 
    public required string FirstName { get; set; } 
    public required string LastName { get; set; } 
    public required string CellNumber { get; set; } 
    public bool? DigiPlusCustomer { get; set; }
    public string? PostCode { get; set; } 
    public required string Address { get; set; } 
}