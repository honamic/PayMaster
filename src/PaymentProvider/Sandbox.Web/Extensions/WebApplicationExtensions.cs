using Microsoft.AspNetCore.Builder;

namespace Honamic.PayMaster.PaymentProvider.Sandbox.Web.Extensions;
public static class WebApplicationExtensions
{
    public const string DefaultSandboxPath = "/paymaster/sandbox/pay";
    public static WebApplication UseSandboxPayEndpoints(this WebApplication app, string sandboxPath = DefaultSandboxPath)
    {
        app.MapRazorPages();

        if (sandboxPath != DefaultSandboxPath)
        {
            app.MapFallbackToPage(DefaultSandboxPath, sandboxPath);
        }

        return app;
    }
}