using Honamic.Framework.Queries;
using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.ReceiptRequests;

public class ReceiptRequestTryLogQueryModel : EntityQueryBase<long>
{
    public long ReceiptRequestId { get; set; }

    public DateTimeOffset CreateAt { get; set; }

    public long? ReceiptRequestGatewayPaymentId { get; set; }

    public ReceiptRequestTryLogType TryType { get; set; }

    public PaymentProviderLogData Data { get; set; }

    public bool Success { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }
}