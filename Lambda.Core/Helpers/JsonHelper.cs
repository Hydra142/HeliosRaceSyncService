using Newtonsoft.Json;

namespace Lambda.Core.Helpers;

public static class JsonHelper
{
    public static async Task<string?> SerializeAsync(object? source)
    {
        if (source is null)
        {
            return null;
        }

        return await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(source);
        });
    }

    public static async Task<T?> DeserializeAsync<T>(string? source)
    {
        if (source is null)
        {
            return default;
        }

        return await Task.Run(() =>
        {
            return JsonConvert.DeserializeObject<T>(source);
        });
    }
}
