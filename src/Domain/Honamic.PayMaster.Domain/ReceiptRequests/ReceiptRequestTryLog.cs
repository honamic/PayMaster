using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.ReceiptRequests.Enums;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domain.ReceiptRequests;

public class ReceiptRequestTryLog : Entity<long>
{
    public ReceiptRequestTryLog()
    {
        Data = new PaymentProviderLogData();
    }

    public long ReceiptRequestId { get; set; }

    public DateTimeOffset CreateAt { get; set; }

    public long? ReceiptRequestGatewayPaymentId { get; set; }

    public ReceiptRequestTryLogType TryType { get; set; }

    public PaymentProviderLogData Data { get; set; }

    public bool Success { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }

    internal void SetSuccess(bool success, PaymentProviderLogData data)
    {
        Success = success;
        Data = data;
    }
}