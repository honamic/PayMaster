using Microsoft.AspNetCore.Http;
using System.Dynamic;
using System.Text.Json;

namespace Honamic.PayMaster.WebApi.Helpers;

public static class ExtractDataHelper
{
    public static string ExtractCallback(this HttpContext context)
    {
        dynamic queryObject = new ExpandoObject();
        var queryObjectDict = (IDictionary<string, object>)queryObject;

        if (HttpMethods.IsPost(context.Request.Method))
        {
            foreach (var param in context.Request.Form)
            {
                queryObjectDict[param.Key] = param.Value.ToString();
            }
        }
        else
        {
            foreach (var param in context.Request.Query)
            {
                queryObjectDict[param.Key] = param.Value.ToString();
            }
        }

        var callbackData = JsonSerializer.Serialize(queryObjectDict);

        return callbackData;
    }
}