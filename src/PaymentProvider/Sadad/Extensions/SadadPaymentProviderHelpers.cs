using Honamic.PayMaster.PaymentProvider.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace Honamic.PayMaster.PaymentProvider.Sadad.Extensions;

public static class SadadPaymentProviderHelpers
{
    public static string CreateSign(ParamsForPayRequest request, SadadConfigurations configurations)
    {
        var data = $"{configurations.TerminalId};{request.UniqueRequestId};{request.Amount}";

        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var tripleDes = TripleDES.Create();
        tripleDes.Mode = CipherMode.ECB;
        tripleDes.Padding = PaddingMode.PKCS7;

        var encryptor = tripleDes.CreateEncryptor(
            Convert.FromBase64String(configurations.MerchantKey), new byte[8]);

        var bytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

        return Convert.ToBase64String(bytes);
    }
}