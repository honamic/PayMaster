using Honamic.Framework.Applications.Authorizes;

namespace WebSample;

internal class DefaultAuthorization : IAuthorization
{
    public bool HaveAccess(string permission)
    {
        return true;
    }

    public Task<bool> HaveAccessAsync(string permission)
    {
        return Task.FromResult(true);
    }

    public bool IsAuthenticated()
    {
        return true;
    }
}