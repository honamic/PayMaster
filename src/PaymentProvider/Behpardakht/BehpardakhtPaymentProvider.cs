using Honamic.PayMaster.Enums;
using Honamic.PayMaster.PaymentProvider.Behpardakht.Dtos;
using Honamic.PayMaster.PaymentProviders;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.Behpardakht;
public class BehpardakhtPaymentProvider : PaymentGatewayProviderBase
{
    private const string ParamsForGatewayPath = "/pgwchannel/services/pgw";
    private readonly ILogger<BehpardakhtPaymentProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private BehpardakhtConfigurations Configurations = new BehpardakhtConfigurations();

    public BehpardakhtPaymentProvider(ILogger<BehpardakhtPaymentProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public override void Configure(string jsonConfiguration)
    {
        var options = JsonSerializer.Deserialize<BehpardakhtConfigurations>(jsonConfiguration);

        if (options == null)
        {
            throw new ArgumentNullException(nameof(jsonConfiguration));
        }

        Configurations = options;
    }

    public override async Task<CreateResult> CreateAsync(CreateRequest request)
    {
        var result = new CreateResult();

        try
        {
            var url = new Uri(new Uri(Configurations.ApiAddress), ParamsForGatewayPath);

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var apiRequest = new bpPayRequestBody
            {
                terminalId = Configurations.TerminalId,
                amount = (long)request.Amount,
                callBackUrl = request.CallbackUrl,
                orderId = request.UniqueRequestId,
            };

            result.LogData.Start(apiRequest, url.ToString());

            var apiResponse = await client.PostAsJsonAsync(url, apiRequest);

            result.LogData.End();

            var rawResponse = await apiResponse.Content.ReadAsStringAsync();

            result.LogData.SetMessage(rawResponse);

            if (apiResponse.IsSuccessStatusCode)
            {
                var mellatResponse = JsonSerializer.Deserialize<MellatPrepareResponse>(rawResponse);
                result.PayUrl = Configurations.PayUrl;
                result.PayVerb = PayVerb.Post;
                result.PayParams.Add("RefId", mellatResponse?.RefId ?? "null");
                result.Success = true;
            }
            else
            {

            }
        }
        catch (Exception ex)
        {
            result.LogData.SetException(ex);
            _logger.LogError(ex, "PrePay Failed");
        }

        return result;
    }

    public override ExtractCallBackDataResult ExtractCallBackData(string callBackJsonValue)
    {
        var result = new ExtractCallBackDataResult();

        try
        {
            var callbackData = JsonSerializer.Deserialize<CallBackDataModel>(callBackJsonValue);

            if (!string.IsNullOrEmpty(callbackData?.SaleOrderId))
            {
                result.UniqueRequestId = long.Parse(callbackData.SaleReferenceId ?? "");
                result.CreateReference = callbackData.RefId;
                result.CallBack = callbackData;
                result.Success = true;
            }
            else
            {
                result.Error = "SaleOrderId value not found.";
            }
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    public override async Task<VerifyResult> VerifyAsync(VerifyRequest request)
    {
        var result = new VerifyResult();

        try
        {
            var callbackData = (CallBackDataModel?)request.CallBackData;

            if (!InternalVerify(request, result, callbackData))
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.InternalVerify;
                return result;
            }
            var resCode = callbackData?.ResCode ?? "";

            if (resCode != "0")
            {
                switch (resCode)
                {
                    case "17":
                        result.PaymentFailedReason = PaymentGatewayFailedReason.Canceled;
                        result.StatusDescription = ResultCodeDescription(resCode);
                        break;
                    default:
                        result.PaymentFailedReason = PaymentGatewayFailedReason.Other;
                        result.StatusDescription = ResultCodeDescription(resCode);
                        break;
                }
                return result;
            }

            var apiVerifyRequest = new bpVerifyRequestBody
            {
                terminalId = Configurations.TerminalId,
                userName = Configurations.UserName,
                userPassword = Configurations.Password,

                orderId = request.PaymentInfo.UniqueRequestId,
                saleOrderId = request.PaymentInfo.UniqueRequestId,
                saleReferenceId = long.Parse(callbackData?.SaleReferenceId!),
            };

            var client = CreateClient();

            result.VerifyLogData.Start(apiVerifyRequest, "bpVerifyRequestAsync()");

            var verifyResponse = await client.bpVerifyRequestAsync(apiVerifyRequest.terminalId, apiVerifyRequest.userName,
                apiVerifyRequest.userPassword, apiVerifyRequest.orderId, apiVerifyRequest.saleOrderId, apiVerifyRequest.saleReferenceId);

            result.VerifyLogData.SetResponse(verifyResponse.Body.@return);

            var VerifyResultCode = verifyResponse.Body.@return;

            if (VerifyResultCode != "0")
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Verify;
                result.StatusDescription = ResultCodeDescription(VerifyResultCode);
                return result;
            }

            result.StartSettlement();
            result.SettlementLogData!.Start(apiVerifyRequest, "bpSettleRequestAsync()");

            var settlmentResponse = await client.bpSettleRequestAsync(apiVerifyRequest.terminalId, apiVerifyRequest.userName,
                apiVerifyRequest.userPassword, apiVerifyRequest.orderId, apiVerifyRequest.saleOrderId, apiVerifyRequest.saleReferenceId);

            result.SettlementLogData.SetResponse(settlmentResponse.Body.@return);

            var settlmentResultCode = verifyResponse.Body.@return;

            if (settlmentResultCode != "0")
            {
                result.PaymentFailedReason = PaymentGatewayFailedReason.Settlement;
                result.StatusDescription = ResultCodeDescription(VerifyResultCode);
                return result;
            }

            result.SupplementaryPaymentInformation = new SupplementaryPaymentInformation
            {
                Pan = callbackData?.CardHolderPan,
                TerminalId = Configurations.TerminalId.ToString(),
            };

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.StatusDescription = ex.Message;
            _logger.LogError(ex, "ExtractCallBackData Failed");
        }

        return result;
    }

    private static bool InternalVerify(VerifyRequest request, VerifyResult result, CallBackDataModel? callbackData)
    {
        if (callbackData is null)
        {
            result.StatusDescription = "Call Back is empty";
            return false;
        }

        // todo: invariant Cultuer fix!
        if (callbackData.FinalAmount != request.PaymentInfo.Amount.ToString())
        {
            result.StatusDescription = "مغایرت در مبلغ";
            return false;
        }

        if (callbackData.RefId != request.PaymentInfo.CreateReference)
        {
            result.StatusDescription = "مغایرت در RefId";
            return false;
        }

        if (callbackData.SaleOrderId
                != request.PaymentInfo.UniqueRequestId.ToString(CultureInfo.InvariantCulture))
        {
            result.StatusDescription = "مغایرت در شناسه درخواست";
            return false;
        }


        return true;
    }

    private PaymentGatewayClient CreateClient()
    {
        return new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort,
            Configurations.ApiAddress);
    }

    public static string ResultCodeDescription(string result) => result switch
    {
        "0" => "تراكنش با موفقيت انجام شد",
        "11" => "شماره كارت نامعتبر است",
        "12" => "موجودي كافي نيست",
        "13" => "رمز نادرست است",
        "14" => "تعداد دفعات وارد كردن رمز بيش از حد مجاز است",
        "15" => "كارت نامعتبر است",
        "16" => "دفعات برداشت وجه بيش از حد مجاز است",
        "17" => "كاربر از انجام تراكنش منصرف شده است",
        "18" => "تاريخ انقضاي كارت گذشته است",
        "19" => "مبلغ برداشت وجه بيش از حد مجاز است",
        "111" => "صادر كننده كارت نامعتبر است",
        "112" => "خطاي سوييچ صادر كننده كارت",
        "113" => "پاسخي از صادر كننده كارت دريافت نشد",
        "114" => "دارنده كارت مجاز به انجام اين تراكنش نيست",
        "21" => "پذيرنده نامعتبر است",
        "23" => "خطاي امنيتي رخ داده است",
        "24" => "اطلاعات كاربري پذيرنده نامعتبر است",
        "25" => "مبلغ نامعتبر است",
        "31" => "پاسخ نامعتبر است",
        "32" => "فرمت اطلاعات وارد شده صحيح نمي باشد",
        "33" => "حساب نامعتبر است",
        "34" => "خطاي سيستمي",
        "35" => "تاريخ نامعتبر است",
        "41" => "شماره درخواست تكراري است",
        "42" => "تراکنش Sale یافت نشد",
        "43" => "قبلا درخواست Verify داده شده است",
        "44" => "درخواست Verify یافت نشد",
        "45" => "تراکنش Settle شده است",
        "46" => "تراکنش Settle نشده است",
        "47" => "تراکنش Settle یافت نشد",
        "48" => "تراکنش Reverse شده است",
        "49" => "تراکنش Refund یافت نشد",
        "412" => "شناسه قبض نادرست است",
        "413" => "شناسه پرداخت نادرست است",
        "414" => "سازمان صادر كننده قبض نامعتبر است",
        "415" => "زمان جلسه كاري به پايان رسيده است",
        "416" => "خطا در ثبت اطلاعات",
        "417" => "شناسه پرداخت كننده نامعتبر است",
        "418" => "اشكال در تعريف اطلاعات مشتري",
        "419" => "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است",
        "421" => "IP نامعتبر است",
        "51" => "تراكنش تكراري است",
        "54" => "تراكنش مرجع موجود نيست",
        "55" => "تراكنش نامعتبر است",
        "61" => "خطا در واريز",
        _ => $"UnexpectedError, Response: {result}"
    };
}
