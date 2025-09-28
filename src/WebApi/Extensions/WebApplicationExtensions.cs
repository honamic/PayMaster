using Honamic.PayMaster.Application;
using Honamic.PayMaster.WebApi.ReceiptRequests;
using Microsoft.AspNetCore.Builder;

namespace Honamic.PayMaster.WebApi.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplication MapPayMasterEndpoints(this WebApplication app, string prefixRoute = PayMasterConstants.WebPrefixRoute)
    {
        ReceiptRequestsEndpoints.MapReceiptRequestsEndpoints(app, prefixRoute);
        return app;
    }
}