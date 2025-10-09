using Honamic.Framework.Endpoints.Web.Results;
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.PaymentGatewayProviders.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Honamic.PayMaster.WebApi.Management.PaymentGatewayProviders;

public static class PaymentGatewayProvidersManagementApiEndpoints
{
    public static void MapPaymentGatewayProvidersManagementApi(this IEndpointRouteBuilder endpoints, string prefixRoute)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        prefixRoute = prefixRoute + "payment-gateway-providers";

        var routeGroup = endpoints.MapGroup(prefixRoute)
            .WithTags("PaymentGatewayProviders")
            //.RequireAuthorization()
            ;

        routeGroup.MapGet("/",
            async Task<IResult> (
                [AsParameters] GetAllLoaddedPaymentGatewayProvidersQuery query,
                [FromServices] IQueryBus queryBus,
                CancellationToken cancellationToken) =>
            {
                var result = await queryBus.DispatchAsync(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName(nameof(GetAllLoaddedPaymentGatewayProvidersQuery))
            .WithOpenApi();

        routeGroup.MapGet("/default-configuration-by-provider-type",
              async Task<IResult> (
                [AsParameters] GetDefaultConfigurationByProviderTypeQuery query,
                  [FromServices] IQueryBus queryBus,
                  CancellationToken cancellationToken) =>
              {
                   var result = await queryBus.DispatchAsync(query, cancellationToken);

                  return result.ToMinimalApiResult();
              })
              .WithName(nameof(GetDefaultConfigurationByProviderTypeQuery))
              .WithOpenApi();
    }
}