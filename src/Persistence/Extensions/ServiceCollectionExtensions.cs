using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptIssuers;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.Persistence.PaymentGatewayProfiles; 
using Honamic.PayMaster.Persistence.ReceiptIssuers;
using Honamic.PayMaster.Persistence.ReceiptRequests;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterPersistenceServices(this IServiceCollection services)
    {
        services.AddTransient<IPaymentGatewayProfileRepository, PaymentGatewayProfileRepository>();
        services.AddTransient<IReceiptIssuerRepository, ReceiptIssuerRepository>();
        services.AddTransient<IReceiptRequestRepository, ReceiptRequestRepository>();
    }

}