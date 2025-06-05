using Honamic.Framework.Domain;
using Honamic.PayMaster.Domains.ReceiptIssuers;
using Honamic.PayMaster.Domains.ReceiptRequests.Enums;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;

namespace Honamic.PayMaster.Domains.ReceiptRequests;

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
    public string Currency { get; set; } = default!;

    public string? Description { get; set; }

    public string? AdditionalData { get; set; }
    public string? Mobile { get; private set; }
    public string? NationalityCode { get; private set; }
    public string? Email { get; private set; }
    public bool? IsLegal { get; private set; }
    public string? IssuerReference { get; private set; }
    public string? PartyReference { get; private set; }

    public long? PartyId { get; private set; }

    public ReceiptIssuer Issuer { get; set; }
    public long IssuerId { get; set; }

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

    public async Task<VerifyResult?> StartCallbackForGatewayPayment(ReceiptRequestGatewayPayment gatewayPayment,
        IPaymentGatewayProvider paymentGatewayProvider,
        string callBackData,
        IClock clock)
    {

        var tryLog = new ReceiptRequestTryLog()
        {
            CreateAt = DateTimeOffset.Now,
            TryType = ReceiptRequestTryLogType.VerifyPaymentProvider,
            ReceiptRequestId = Id,
            ReceiptRequestGatewayPaymentId = gatewayPayment.Id,
        };

        VerifyResult? verifyResult = null;

        try
        {
            gatewayPayment.StartCallback(clock.NowWithOffset, callBackData);

            var extractCallBackDataResult = paymentGatewayProvider.ExtractCallBackData(callBackData);

            if (!extractCallBackDataResult.Success)
            {
                gatewayPayment.FailedCallBack(extractCallBackDataResult.PaymentFailedReason ?? PaymentGatewayFailedReason.Other, extractCallBackDataResult?.Error);
                return null;
            }

            var callbackValidityDuration = paymentGatewayProvider.GetCallbackValidityDuration();

            InternalVerifyCallbackData(gatewayPayment, extractCallBackDataResult, callbackValidityDuration);

            if (gatewayPayment.Status == PaymentGatewayStatus.Failed)
            {
                return null;
            }

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

            verifyResult = await paymentGatewayProvider.VerifyAsync(verifyRequest);

            if (verifyResult.Success)
            {
                gatewayPayment.SuccessCallBack(verifyResult.SupplementaryPaymentInformation);
            }
            else
            {
                gatewayPayment.FailedCallBack(verifyResult.PaymentFailedReason ?? PaymentGatewayFailedReason.Other, verifyResult.StatusDescription);
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
        }
        catch (Exception ex)
        {
            tryLog.Data.SetException(ex);
        }

        TryLogs.Add(tryLog);

        return verifyResult;
    }

    private static void InternalVerifyCallbackData(ReceiptRequestGatewayPayment gatewayPayment,
        ExtractCallBackDataResult extractCallBackDataResult,
        TimeSpan callbackValidityDuration)
    {
        if (extractCallBackDataResult.UniqueRequestId is null
            && string.IsNullOrEmpty(extractCallBackDataResult.CreateReference))
        {
            gatewayPayment.FailedCallBack(PaymentGatewayFailedReason.InternalVerify, $"Neither UniqueRequestId nor CreateReference was provided.");
            return;
        }

        if (extractCallBackDataResult.UniqueRequestId.HasValue
           && gatewayPayment.Id != extractCallBackDataResult.UniqueRequestId)
        {
            gatewayPayment.FailedCallBack(PaymentGatewayFailedReason.InternalVerify, $"ID is not correct.");
            return;
        }

        if (!string.IsNullOrEmpty(extractCallBackDataResult.CreateReference)
             && !string.IsNullOrEmpty(gatewayPayment.CreateReference)
             && !gatewayPayment.CreateReference.Equals
                   (extractCallBackDataResult.CreateReference, StringComparison.InvariantCultureIgnoreCase))
        {
            gatewayPayment.FailedCallBack(PaymentGatewayFailedReason.InternalVerify, $"CreateReference is not correct");
            return;
        }

        if (gatewayPayment.CallbackAt!.Value - gatewayPayment.RedirectAt!.Value > callbackValidityDuration)
        {
            gatewayPayment.FailedCallBack(PaymentGatewayFailedReason.InternalVerify, $"It's late for callback.");
            return;
        }
    }
}