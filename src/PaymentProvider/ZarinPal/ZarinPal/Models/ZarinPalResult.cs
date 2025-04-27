using System.Text.Json.Serialization;
using System.Text.Json;

namespace Honamic.PayMaster.PaymentProvider.ZarinPal.Models;

public class ZarinPalResult<T>
{
    public T? data { get; set; }
    public ZarinPalErrorResultWrapper? errors { get; set; }
}

[JsonConverter(typeof(ErrorConverter))]
public class ZarinPalErrorResultWrapper
{
    public ZarinPalErrorResult[]? errors { get; set; }
}


public class ZarinPalErrorResult
{
    public string message { get; set; }
    public int? code { get; set; }
    public string[]? validations { get; set; }
}



public class ErrorConverter : JsonConverter<ZarinPalErrorResultWrapper?>
{
    public override ZarinPalErrorResultWrapper? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new ZarinPalErrorResultWrapper()
        {

        };

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            result.errors = JsonSerializer.Deserialize<ZarinPalErrorResult[]?>(ref reader, options);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            var errorObject = JsonSerializer.Deserialize<ZarinPalErrorResult?>(ref reader, options);

            result.errors = new ZarinPalErrorResult[] { errorObject };
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ZarinPalErrorResultWrapper? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}