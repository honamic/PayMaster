using Microsoft.Extensions.DependencyInjection;
using Honamic.PayMaster.Persistence.PaymentGatewayProviders;
using Honamic.PayMaster.Persistence.ReceiptIssuers;
using Honamic.PayMaster.Persistence.ReceiptRequests;
using Honamic.PayMaster.QueryModels.PaymentGatewayProviders;
using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Honamic.PayMaster.QueryModels.ReceiptRequests;

namespace Honamic.PayMaster.QueryModels.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterQueryModelServices(this IServiceCollection services)
    {
        services.AddTransient<IPaymentGatewayProviderQueryModelRepository, PaymentGatewayProviderQueryModelRepository>();
        services.AddTransient<IReceiptIssuerQueryModelRepository, ReceiptIssuerQueryModelRepository>();
        services.AddTransient<IReceiptRequestQueryModelRepository, ReceiptRequestRepositoryQueryModel>();
    }
}