using Microsoft.AspNetCore.Builder;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplication UseSandboxPayEndpoints(this WebApplication app, string sandboxPath = "/paymaster/sandbox/pay")
    {
        SandboxEndpoints.MapSandboxPayEndpoints(app, sandboxPath);
        return app;
    }
}