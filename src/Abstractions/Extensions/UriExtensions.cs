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

    /// <summary>
    /// Safely combines a base URI with a relative path
    /// </summary>
    /// <param name="baseUri">The base URI</param>
    /// <param name="relativePath">The relative path to append</param>
    /// <returns>A properly combined URI</returns>
    public static Uri Combine(this Uri baseUri, string relativePath)
    {
        if (baseUri == null)
        {
            throw new ArgumentNullException(nameof(baseUri), "Base URI cannot be null");
        }

        if (string.IsNullOrEmpty(relativePath))
        {
            return baseUri;
        }

        string baseStr = baseUri.ToString();
        if (!baseStr.EndsWith("/"))
        {
            baseStr += "/";
        }

        string relPath = relativePath.TrimStart('/');
        return new Uri(baseStr + relPath);
    }
    
    /// <summary>
    /// Safely combines a base URL string with a relative path
    /// </summary>
    /// <param name="baseUrl">The base URL as a string</param>
    /// <param name="relativePath">The relative path to append</param>
    /// <returns>A properly combined URI</returns>
    public static Uri Combine(string baseUrl, string relativePath)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null or empty");
        }

        return Combine(new Uri(baseUrl), relativePath);
    }

    /// <summary>
    /// Safely appends query parameters to a URI
    /// </summary>
    /// <param name="uri">The base URI</param>
    /// <param name="queryParams">Query parameters in format "name=value"</param>
    /// <returns>URI with query parameters</returns>
    public static Uri AppendQueryParams(this Uri uri, params string[] queryParams)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri), "URI cannot be null");
        }

        if (queryParams == null || queryParams.Length == 0)
        {
            return uri;
        }

        var uriBuilder = new UriBuilder(uri);
        string query = uriBuilder.Query;
        
        if (query.StartsWith("?"))
        {
            query = query[1..];
        }
        
        var queryCollection = new List<string>();
        
        if (!string.IsNullOrEmpty(query))
        {
            queryCollection.Add(query);
        }
        
        queryCollection.AddRange(queryParams.Where(p => !string.IsNullOrEmpty(p)));
        
        uriBuilder.Query = string.Join("&", queryCollection);
        return uriBuilder.Uri;
    }

    /// <summary>
    /// Safely joins URL path segments with proper handling of slashes
    /// </summary>
    /// <param name="segments">The URL path segments to join</param>
    /// <returns>A properly joined URL path</returns>
    public static string JoinUrlSegments(params string[] segments)
    {
        if (segments == null || segments.Length == 0)
        {
            return string.Empty;
        }

        return string.Join("/", segments
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.Trim('/'))
            .Where(s => !string.IsNullOrEmpty(s)));
    }
}