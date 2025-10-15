using Honamic.Framework.Endpoints.Web.Results; 
using Honamic.Framework.Queries;
using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Honamic.PayMaster.WebApi.Management.ReceiptRequests;

public static class ReceiptRequestsManagementApiEndpoints
{
    public static void MapReceiptRequestManagementApi(this IEndpointRouteBuilder endpoints, string prefixRoute)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        prefixRoute = prefixRoute + "receipt-requests";

        var routeGroup = endpoints.MapGroup(prefixRoute)
            .WithTags("ReceiptRequests")
            //.RequireAuthorization()
            ;

        routeGroup.MapGet("/",
            async Task<IResult> (
                [AsParameters] GetAllReceiptRequestsQuery filter,
                [FromServices] IQueryBus queryBus,
                CancellationToken cancellationToken) =>
            {
                var result = await queryBus.DispatchAsync(filter, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName("GetAllReceiptRequests")
            .WithOpenApi(); 
    }
}