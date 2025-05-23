using Honamic.Framework.Applications.Results;
using Honamic.Framework.Commands;

namespace Honamic.PayMaster.Application.ReceiptRequests.Commands;
public class CreateReceiptRequestCommand : ICommand<Result<CreateReceiptRequestCommandResult>>
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;


    public string? GatewayProviderCode { get; set; }
    public long? GatewayProviderId { get; set; }

    public string? Description { get; set; }

    public string? IssuerCode { get; set; }
    public string? IssuerReference { get; set; }

    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? NationalityCode { get; set; }
    public string? PartyReference { get; set; }
    public long? PartyId { get; set; }
    public bool? IsLegal { get; set; }

    public string? AdditionalData { get; set; }
}

public class CreateReceiptRequestCommandResult
{
    public string Id { get; set; }
}