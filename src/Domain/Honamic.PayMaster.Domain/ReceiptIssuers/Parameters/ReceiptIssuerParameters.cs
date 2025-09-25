namespace Honamic.PayMaster.Domain.ReceiptIssuers.Parameters;

public class ReceiptIssuerParameters
{
    public required int Id { get; set; }

    public required string Code { get; set; }

    public required string CallbackUrl { get; set; }

    public required string Title { get; set; }

    public required bool Enabled { get; set; }
    
    public string? Description { get; set; }
}
