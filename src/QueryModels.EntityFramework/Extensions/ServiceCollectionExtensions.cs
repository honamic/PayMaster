using Honamic.PayMaster.QueryModels.EntityFramework.PaymentGatewayProfiles;
using Honamic.PayMaster.QueryModels.EntityFramework.ReceiptIssuers;
using Honamic.PayMaster.QueryModels.EntityFramework.ReceiptRequests;
using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;
using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.QueryModels.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterQueryModelServices(this IServiceCollection services)
    {
        services.AddTransient<IPaymentGatewayProfileQueryModelRepository, PaymentGatewayProfileQueryModelRepository>();
        services.AddTransient<IReceiptIssuerQueryModelRepository, ReceiptIssuerQueryModelRepository>();
        services.AddTransient<IReceiptRequestQueryModelRepository, ReceiptRequestQueryModelRepository>();
    }
}