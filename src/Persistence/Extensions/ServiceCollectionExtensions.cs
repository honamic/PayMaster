using Microsoft.Extensions.DependencyInjection;
using Honamic.PayMaster.Domain.PaymentGatewayProviders;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Persistence.PaymentGatewayProviders;
using Honamic.PayMaster.Persistence.ReceiptIssuers;
using Honamic.PayMaster.Persistence.ReceiptRequests;

namespace Honamic.PayMaster.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterPersistenceServices(this IServiceCollection services)
    {
        services.AddTransient<IPaymentGatewayProviderRepository, PaymentGatewayProviderRepository>();
        services.AddTransient<IReceiptIssuerRepository, ReceiptIssuerRepository>();
        services.AddTransient<IReceiptRequestRepository, ReceiptRequestRepository>();
    }

}