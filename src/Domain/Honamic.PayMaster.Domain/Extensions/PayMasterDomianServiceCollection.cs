using Honamic.PayMaster.Domain.ReceiptRequests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Domain.Extensions;

public static class PayMasterDomianServiceCollection
{
 
    public static IServiceCollection AddPayMasterDomainServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IPaymentGatewayInitializationService, PaymentGatewayInitializationService>();
        services.AddScoped<ICreateReceiptRequestDomainService, CreateReceiptRequestDomainService>();
        services.AddScoped<ICallbackGatewayPaymentDomainService, CallbackGatewayPaymentDomainService>();

        return services;
    }
}