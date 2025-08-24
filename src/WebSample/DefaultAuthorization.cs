using Honamic.Framework.Domain;

namespace WebSample;

internal class DefaultAuthorization : IAuthorization
{
    public Task<bool> HavePermissionAsync(string permission, string? module = null)
    {
        return Task.FromResult(true);
    }

    public Task<bool> HaveRoleAsync(string roleName)
    {
        return Task.FromResult(true);
    }

    public bool IsAuthenticated()
    {
        return true;
    }
}