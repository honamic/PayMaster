using Honamic.PayMaster.PaymentProvider.Sandbox.Extensions;
using Honamic.PayMaster.PaymentProvider.Sandbox.Web.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSandboxWebPaymentProviderServices(this IServiceCollection services)
    {
        services.AddRazorPages() 
                .AddApplicationPart(typeof(SandboxPayModel).Assembly);

        services.AddSandboxPaymentProviderServices();

        return services;
    }
}