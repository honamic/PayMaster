using Honamic.PayMaster.Application;
using Honamic.PayMaster.WebApi.Management.PaymentGatewayProviders;
using Honamic.PayMaster.WebApi.Management.ReceiptRequests;
using Microsoft.AspNetCore.Routing;

namespace Honamic.PayMaster.WebApi.Management.Extensions;

public static class EndpointRouteExtensions
{
    public static IEndpointRouteBuilder MapPayMasterManagementEndpoints(this IEndpointRouteBuilder endpoints, string prefixRoute = PayMasterConstants.ManagementPrefixRoute)
    { 
        endpoints.MapReceiptRequestManagementApi(prefixRoute); 
        endpoints.MapPaymentGatewayProvidersManagementApi(prefixRoute); 

        return endpoints;
    }
}