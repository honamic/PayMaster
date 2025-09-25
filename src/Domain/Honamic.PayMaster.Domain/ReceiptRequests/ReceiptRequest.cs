using Honamic.Framework.Domain;
using Honamic.PayMaster.Domain.ReceiptRequests.Enums;
using Honamic.PayMaster.Domain.ReceiptRequests.Exceptions;
using Honamic.PayMaster.Domain.ReceiptRequests.Parameters; 
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domain.ReceiptRequests;

public class ReceiptRequest : AggregateRoot<long>
{
    public ReceiptRequest()
    {
        GatewayPayments = [];
        TryLogs = [];
    }

    public ReceiptRequestStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public string Currency { get; private set; } = default!;

    public long IssuerId { get; private set; }

    public string? Description { get; private set; }

    public string? AdditionalData { get; private set; }
    public string? Mobile { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public bool? IsLegal { get; private set; }
    public string? IssuerReference { get; private set; }
    public string? PartyReference { get; private set; }

    public long? PartyId { get; private set; }


    public List<ReceiptRequestGatewayPayment> GatewayPayments { get; set; }

    public List<ReceiptRequestTryLog> TryLogs { get; set; }

    public static ReceiptRequest Create(CreateReceiptRequestParameters createParameters,
        IIdGenerator idGenerator)
    {
        if (createParameters.Amount <= 0)
        {
            throw new ArgumentException($"مبلغ نمی تواند صفر یا کوچکتر باشد.");
        }

        if (!createParameters.SupportedCurrencies.Contains(createParameters.Currency))
        {
            throw new ArgumentException($"واحد پولی {createParameters.Currency} مجاز نیست.");
        }

        if (createParameters.Issuer is null)
        {
            throw new ArgumentException($"صادرکننده مشخص نشده است.");
        }
        else if (!createParameters.Issuer.Enabled)
        {
            throw new ArgumentException($"صادرکننده فیش غیرفعال است.");
        }

        var newReceiptRequest = new ReceiptRequest()
        {
            Id = idGenerator.GetNewId(),
            Status = ReceiptRequestStatus.New,
            Amount = createParameters.Amount,
            Currency = createParameters.Currency,

            AdditionalData = createParameters.AdditionalData,
            Email = createParameters.Email,
            Description = createParameters.Description,
            IsLegal = createParameters.IsLegal,
            Mobile = createParameters.Mobile,

            PartyId = createParameters.PartyId,
            PartyReference = createParameters.PartyReference,
            IssuerReference = createParameters.IssuerReference,
            NationalityCode = createParameters.NationalityCode,
            IssuerId = createParameters.Issuer.Id,
        };

        CreateGatewayPaymentParameters createGatewayPayment = new CreateGatewayPaymentParameters
        {
            Id = idGenerator.GetNewId(),
            Amount = createParameters.Amount,
            Currency = createParameters.Currency,
            GatewayProvider = createParameters.GatewayProvider,
        };

        ReceiptRequestGatewayPayment newPayment = ReceiptRequestGatewayPayment.Create(createGatewayPayment);

        newReceiptRequest.GatewayPayments.Add(newPayment);

        return newReceiptRequest;
    }

    public ReceiptRequestGatewayPayment? GetPayableGatewayPayment()
    {
        return GatewayPayments.FirstOrDefault(c => c.Status == PaymentGatewayStatus.New);
    }

    public ReceiptRequestGatewayPayment? GetGatewayPayment(long id)
    {
        return GatewayPayments.FirstOrDefault(c => c.Id == id);
    }

    public async Task<VerifyResult?> VerifyGatewayPayment(ReceiptRequestGatewayPayment gatewayPayment,
        IPaymentGatewayProvider paymentGatewayProvider,
        ExtractCallBackDataResult extractCallBackDataResult)
    {
        var tryLog = new ReceiptRequestTryLog()
        {
            CreateAt = DateTimeOffset.Now,
            TryType = ReceiptRequestTryLogType.VerifyPaymentProvider,
            ReceiptRequestId = Id,
            ReceiptRequestGatewayPaymentId = gatewayPayment.Id,
        };
        TryLogs.Add(tryLog);

        VerifyRequest verifyRequest = new()
        {
            PaymentInfo = new VerifyRequestPatmentInfo
            {
                Amount = gatewayPayment!.Amount,
                UniqueRequestId = gatewayPayment.Id,
                CreateReference = gatewayPayment.CreateReference,
            },
            CallBackData = extractCallBackDataResult.CallBack
        };

        var verifyResult = await paymentGatewayProvider.VerifyAsync(verifyRequest);

        if (verifyResult.Success)
        {
            gatewayPayment.SuccessVerify(verifyResult.SupplementaryPaymentInformation);
        }
        else
        {
            gatewayPayment.FailedVerify(verifyResult.PaymentFailedReason ?? PaymentGatewayFailedReason.Other, verifyResult.StatusDescription);
        }

        tryLog.Success = verifyResult.Success;
        tryLog.Data = verifyResult.VerifyLogData;

        if (verifyResult.SettlementLogData is not null)
        {
            TryLogs.Add(new ReceiptRequestTryLog
            {
                CreateAt = DateTimeOffset.Now,
                TryType = ReceiptRequestTryLogType.SettlementPaymentProvider,
                ReceiptRequestId = Id,
                ReceiptRequestGatewayPaymentId = gatewayPayment.Id,
                Data = verifyResult.SettlementLogData,
            });
        }

        return verifyResult;
    }

    public bool InternalVerifyCallbackData(ReceiptRequestGatewayPayment gatewayPayment,
        ExtractCallBackDataResult extractCallBackDataResult,
        TimeSpan callbackValidityDuration)
    {
        if (extractCallBackDataResult.UniqueRequestId is null
            && string.IsNullOrEmpty(extractCallBackDataResult.CreateReference))
        {
            gatewayPayment.FailedCallback(PaymentGatewayFailedReason.InternalVerify, $"Neither UniqueRequestId nor CreateReference was provided.");
            return false;
        }

        if (extractCallBackDataResult.UniqueRequestId.HasValue
           && gatewayPayment.Id != extractCallBackDataResult.UniqueRequestId)
        {
            gatewayPayment.FailedCallback(PaymentGatewayFailedReason.InternalVerify, $"ID is not correct.");
            return false;
        }

        if (!string.IsNullOrEmpty(extractCallBackDataResult.CreateReference)
             && !string.IsNullOrEmpty(gatewayPayment.CreateReference)
             && !gatewayPayment.CreateReference.Equals
                   (extractCallBackDataResult.CreateReference, StringComparison.InvariantCultureIgnoreCase))
        {
            gatewayPayment.FailedCallback(PaymentGatewayFailedReason.InternalVerify, $"CreateReference is not correct");
            return false;
        }

        if (gatewayPayment.CallbackAt!.Value - gatewayPayment.RedirectAt!.Value > callbackValidityDuration)
        {
            gatewayPayment.FailedCallback(PaymentGatewayFailedReason.InternalVerify, $"It's late for callback.");
            return false;
        }

        return true;
    }

    public void InitializeGatewayPayment()
    {
        if (Status != ReceiptRequestStatus.New
            && Status != ReceiptRequestStatus.Doing)
        {
            throw new StatusNotValidForInitializeException();
        }

        Status = ReceiptRequestStatus.Doing;
    }

    internal void EnsureCanProcessCallback()
    {
        if (Status != ReceiptRequestStatus.Doing)
        {
            throw new PaymentStatusNotValidForProcessingException();
        }

    }
    internal void UpdateStatusAfterVerifyGatewayPayment()
    {
        var sumOfSuccess = GatewayPayments
           .Where(c => c.Status == PaymentGatewayStatus.Success)
           .Sum(c => c.Amount);

        if (sumOfSuccess == Amount)
        {
            Status = ReceiptRequestStatus.Done;
        }
    }
}