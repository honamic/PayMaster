using Microsoft.AspNetCore.Builder;

namespace Honamic.PayMaster.Web.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplication UsePayMasterEndpoints(this WebApplication app)
    {
        ReceiptRequestsEndpoints.MapReceiptRequestsEndpoints(app);
        return app;
    }
}