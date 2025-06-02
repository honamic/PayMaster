namespace Honamic.PayMaster.Extensions;
public static class UriExtensions
{
    public static string GetOrigin(Uri? uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri), "URI cannot be null when extracting host");
        }

        return $"{uri.Scheme}://{uri.Host}";
    }
}