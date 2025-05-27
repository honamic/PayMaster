using Microsoft.Extensions.DependencyInjection;
using Honamic.Framework.Tools.IdGenerators;
using Honamic.Framework.Applications.Extensions;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
using Honamic.PayMaster.PaymentProviders;
using Honamic.Framework.Applications.Results;
using Honamic.PayMaster.Domains.ReceiptRequests.Services;

namespace Honamic.PayMaster.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPayMasterServices(this IServiceCollection services)
    {
        services.AddDomainServices();
        services.AddApplicationServices();
        services.AddSnowflakeIdGeneratorServices();
    }

    private static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ICreatePaymentDomainService, CreatePaymentDomainService>();
        services.AddScoped<ICreateReceiptRequestDomainService, CreateReceiptRequestDomainService>();
        services.AddScoped<ICallbackGatewayPaymentDomainService, CallbackGatewayPaymentDomainService>();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddDefaultApplicationsServices();

        services.AddSingleton<IPaymentGatewayProviderFactory, PaymentGatewayProviderFactory>();

        services.AddCommandHandler<CreateReceiptRequestCommand,
            CreateReceiptRequestCommandHandler,
            Result<CreateReceiptRequestCommandResult>>();

        services.AddCommandHandler<PayReceiptRequestCommand,
            PayReceiptRequestCommandHandler,
            Result<PayReceiptRequestCommandResult>>();

        services.AddCommandHandler<CallBackGatewayPaymentCommand,
            CallBackGatewayPaymentCommandHandler,
            Result<CallBackGatewayPaymentCommandResult>>();
    }
}