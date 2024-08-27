using System.Text.Json;
using System.Text.Json.Serialization;

namespace DDNS.Core.Utils;

public static class JsonSerializerHelper
{
    public static readonly JsonSerializerOptions JsonSerializerSettings = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string Serialize<T>(T obj)
        => JsonSerializer.Serialize(obj, JsonSerializerSettings);

    public static T? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json, JsonSerializerSettings);
}
