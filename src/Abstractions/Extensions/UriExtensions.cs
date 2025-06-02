namespace Honamic.PayMaster.Extensions;
public static class UriExtensions
{
    public static string GetOrigin(this Uri? uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri), "URI cannot be null when extracting host");
        }

        return $"{uri.Scheme}://{uri.Host}:{uri.Port}".ToLowerInvariant();
    }
}