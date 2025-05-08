using System.Text.Json;

namespace ModulesApp.Helpers;

public class DataConvertor
{
    public static List<T?> ToList<T>(object? value)
    {
        var targetType = typeof(T);
        if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
        {
            var result = new List<T?>(json.GetArrayLength());
            
            foreach (var element in json.EnumerateArray())
            {
                if (targetType == typeof(string))
                {
                    result.Add((T)(object)ToString(element));
                }
                else if (targetType == typeof(double))
                {
                    result.Add((T)(object)ToDouble(element));
                }
                else if (targetType == typeof(bool))
                {
                    result.Add((T)(object)ToBool(element));
                }
                else
                {
                    result.Add(default);
                }
            }
            return result;
        }
        return [];
    }

    public static string ToString(object? value)
    {
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.String)
            {
                return json.GetString() ?? string.Empty;
            }
            return json.ToString();
        }
        return value?.ToString() ?? string.Empty;
    }

    public static double ToDouble(object? value)
    {
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.GetDouble();
            }
            else if (json.ValueKind == JsonValueKind.String && double.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
        }
        if (value is string str && double.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        return default;
    }

    public static bool ToBool(object? value)
    {
        if (value is JsonElement json)
        {
            if(json.ValueKind == JsonValueKind.True)
            {
                return true;
            }
            else if (json.ValueKind == JsonValueKind.False)
            {
                return false;
            }
            else if (json.ValueKind == JsonValueKind.String && bool.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
            else if (json.ValueKind == JsonValueKind.Number)
            {
                if (json.GetDouble() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        if (value is string str && bool.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        if (value is double i)
        {
            return i != 0;
        }
        return false;
    }

    //public static JsonElement ToJsonArray(object? value)
    //{
    //    if (value is JsonElement json)
    //    {
    //        if (json.ValueKind == JsonValueKind.Array)
    //        {
    //            return json;
    //        }
    //        else
    //        {
    //            return JsonSerializer.SerializeToElement(new List<object?> { value });
    //        }
    //    }
    //    if (value is List<object?> list)
    //    { 
    //        return JsonSerializer.SerializeToElement(list);
    //    }
    //    return JsonSerializer.SerializeToElement(new List<object?> { value });
    //}
}
