using Honamic.Framework.Applications.Authorizes;
using Honamic.Framework.Applications.Extensions;
using Honamic.Framework.Applications.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.CommandHandlers;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Commands;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProfiles.QueryHandlers;
using Honamic.PayMaster.Application.PaymentGatewayProviders;
using Honamic.PayMaster.Application.PaymentProviders;
using Honamic.PayMaster.Application.ReceiptRequests.CommandHandlers;
using Honamic.PayMaster.Application.ReceiptRequests.Commands;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.QueryHandlers;
using Honamic.PayMaster.PaymentProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Application.Extensions;

public static class PayMasterApplicationServiceCollection
{
    public static IServiceCollection AddPayMasterApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IPaymentGatewayProviderFactory, PaymentGatewayProviderFactory>();
        services.AddSingleton<IBearerTokensStore, InMemoryBearerTokensStore>();


        DynamicPermissionRegistry.Register(typeof(PayMasterApplicationServiceCollection).Assembly);
        DynamicPermissionRegistry.Register(typeof(PayMasterConstants).Assembly);

        services.AddCommandHandlers();
        services.AddQueryHandlers();
        services.AddEventHandlers();

        return services;
    }

    private static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddCommandHandler<CreateReceiptRequestCommand,
            CreateReceiptRequestCommandHandler,
            Result<CreateReceiptRequestCommandResult>>();

        services.AddCommandHandler<InitiatePayReceiptRequestCommand,
            InitiatePayReceiptRequestCommandHandler,
            Result<InitiatePayReceiptRequestCommandResult>>();

        services.AddCommandHandler<CallBackGatewayPaymentCommand,
            CallBackGatewayPaymentCommandHandler,
            Result<CallBackGatewayPaymentCommandResult>>();

        services.AddCommandHandler<RepayReceiptRequestCommand,
            RepayReceiptRequestCommandHandler,
            Result<RepayReceiptRequestCommandResult>>();

        services.AddCommandHandler<UpdatePaymentGatewayProfileCommand,
            UpdatePaymentGatewayProfileCommandHandler,
            Result<UpdatePaymentGatewayProfileCommandResult>>();

        services.AddCommandHandler<CreatePaymentGatewayProfileCommand,
            CreatePaymentGatewayProfileCommandHandler,
            Result<CreatePaymentGatewayProfileCommandResult>>();
    }

    private static void AddQueryHandlers(this IServiceCollection services)
    {
        services.AddQueryHandler<GetPublicReceiptRequestQuery,
            Result<GetPublicReceiptRequestQueryResult?>,
            GetPublicReceiptRequestQueryHandler>();

        services.AddQueryHandler<GetActivePaymentGatewaysQuery,
            Result<List<GetActivePaymentGatewaysQueryResult>>,
            GetActivePaymentGatewaysQueryHandler>();

        services.AddQueryHandler<GetAllPaymentGatewaysQuery,
            Result<PagedQueryResult<GetAllPaymentGatewaysQueryResult>>,
            GetAllPaymentGatewaysQueryHandler>();

        services.AddQueryHandler<GetPaymentGatewayQuery,
            Result<GetPaymentGatewayQueryResult>,
            GetPaymentGatewayQueryHandler>();

        services.AddQueryHandler<GetAllLoaddedPaymentGatewayProvidersQuery,
            Result<List<GetAllLoaddedPaymentGatewayProvidersQueryResult>>,
            GetAllLoaddedPaymentGatewayProvidersQueryHandler>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        //services.AddEventHandler<UserCreatedEvent, UserCreatedEventHandler>();
    }
}