using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProvider.Sandbox.Models;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.Sandbox;

public class SandboxPaymentProvider(ILogger<SandboxPaymentProvider> logger) : PaymentGatewayProviderBase<SandboxConfigurations>
{
    public override Task<CreateResult> CreateAsync(CreateRequest createRequest)
    {
        var createResult = new CreateResult
        {
            Success = true,
            CreateReference = DateTime.Now.Ticks.ToString(),
            PayUrl = Configurations.PayUrl,
            PayVerb = PayVerb.Get
        };


        var createSandboxRequest = new SanboxRequestDataModel
        {
            PayId = createResult.CreateReference,
            Amount = (long)createRequest.Amount,
            Token = HashCode(CreateToken(createRequest.Amount, createRequest.Currency)),
            Currency = createRequest.Currency,
            CallbackUrl = createRequest.CallbackUrl,
            GatewayNote = createRequest.GatewayNote ?? string.Empty,
            UniqueRequestId = createRequest.UniqueRequestId.ToString(),
        };

        createResult.PayParams.Add("payId", createSandboxRequest.PayId);
        createResult.PayParams.Add("token", createSandboxRequest.Token);
        createResult.PayParams.Add("amount", createSandboxRequest.Amount.ToString());
        createResult.PayParams.Add("currency", createSandboxRequest.Currency ?? string.Empty);
        createResult.PayParams.Add("callbackUrl", createSandboxRequest.CallbackUrl);
        createResult.PayParams.Add("GatewayNote", createSandboxRequest.GatewayNote ?? "");
        createResult.PayParams.Add("uniqueRequestId", createSandboxRequest.UniqueRequestId);

        createResult.LogData.Start(createSandboxRequest, "");
        createResult.LogData.SetResponse(new { createResult.PayUrl, createResult.PayParams });

        return Task.FromResult(createResult);

    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<SanboxCallBackDataModel>(callBackJsonValue, new JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            });

            if (callbackData is { Status: "OK" })
            {
                result.UniqueRequestId = long.Parse(callbackData.PayRequestId ?? "0");
                result.CreateReference = callbackData.PayId;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else if (callbackData is { Status: "NOK" })
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Canceled;
            }
            else
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Other;
                result.Error = $"Status is not valid! [{callbackData?.Status}]";
            }
        }
        catch (Exception ex)
        {
            result.Error = "extract callback data failed.";
            logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerifyResult();
        try
        {
            var callbackData = (SanboxCallBackDataModel?)request.CallBackData;

            if (callbackData.Status != "OK")
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Canceled;
                return Task.FromResult(result);
            }

            if (callbackData.Amount == null || callbackData.Amount <= 0)
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                result.StatusDescription = "callback data not valid";
                return Task.FromResult(result);
            }

            if (callbackData.Currency == null || callbackData.Currency != "IRR")
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                result.StatusDescription = "callback data not valid";
                return Task.FromResult(result);
            }

            result.VerifyLogData.Start(callbackData, "");

            var VerifyResult = CreateToken(callbackData.Amount, callbackData.Currency);

            result.VerifyLogData.SetResponse(new { VerifyResult });

            if (VerifyResult != callbackData.Token)
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                result.StatusDescription = "callback data not valid";
                return Task.FromResult(result);
            }


            result.Success = true;
            result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
            {
                Pan = callbackData.Pan,
                SuccessReference = callbackData.TrackingNumber,
                ReferenceRetrievalNumber = null,
                TrackingNumber = callbackData.TrackingNumber,
                TerminalId = "123456",
                MerchantId = "1234567890",
            };
        }
        catch (Exception ex)
        {
            result.StatusDescription = ex.Message;
            logger.LogError(ex, "Verify Failed");
        }

        return Task.FromResult(result);
    }

    private static string CreateToken(decimal amount, string currency)
    {
        return HashCode($"{amount}|{currency}");
    }

    private static string HashCode(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}