using System.Web;

namespace Honamic.PayMaster.Web.Helpers;
public static class UrlHelper
{
    public static string AddParametersToUrl(this string baseUrl, Dictionary<string, string>? parameters)
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (parameters is not null)
            foreach (var param in parameters)
            {
                query[param.Key] = param.Value;
            }

        uriBuilder.Query = query.ToString();

        return uriBuilder.ToString();
    }
}