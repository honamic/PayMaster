using Honamic.Framework.Domain;
using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.Domains.ReceiptRequests.Parameters;
using Honamic.PayMaster.Enums;

namespace Honamic.PayMaster.DomainTests.ReceiptRequests.Helper;
public static class ReceiptRequestsHelper
{
    public static ReceiptRequest CreateSampleReceiptRequest(ReceiptRequestStatus status, IIdGenerator _idGenerator)
    {
        var receiptRequest = ReceiptRequest.Create(new CreateReceiptRequestParameters
        {
            Amount = 10000,
            Currency = "IRR",
            Description = "Test payment",
            DefaultGatewayProviderCode = "sandbox",
            DefaultIssuerCode = "default",
            IssuerCode = "",
            SupportedCurrencies = ["IRR", "USD"],
            Mobile = "09123456789",
            Email = "test@example.com",
            Issuer = new ReceiptRequestIssuerParameters
            {
                Id = 1, // Use the ID of the created issuer
                Enabled = true
            },
            GatewayProvider = new ReceiptRequestGatewayProviderParameters
            {
                Enabled = true,
                Id = 789
            }
        }, idGenerator: _idGenerator);

        if (status == ReceiptRequestStatus.Doing)
        {
            typeof(ReceiptRequest)
                 .GetProperty(nameof(ReceiptRequest.Status))
                 ?.SetValue(receiptRequest, status);
        }

        return receiptRequest;
    }

}
