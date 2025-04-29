using System.Text.Json;

namespace ModulesApp.Helpers;

public class DataConvertor
{
    public static List<T>? ToList<T>(object? value)
    {
        if (value is List<T> list)
        {
            return list;
        }
        if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
        {
            var result = new List<T>(json.GetArrayLength());
            var targetType = typeof(T);

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
                    throw new InvalidOperationException($"Unsupported list element type {typeof(T)}");
                }
            }
            return result;
        }
        return null;
    }

    public static string ToString(object? value)
    {
        if (value is string str)
        {
            return str;
        }
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.String)
            {
                return json.GetString() ?? string.Empty;
            }
            return json.ToString();
        }
        return string.Empty;
    }

    public static double ToDouble(object? value)
    {
        if (value is double d)
        {
            return d;
        }
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
        return 0;
    }

    public static bool ToBool(object? value)
    {
        if (value is bool b)
        {
            return b;
        }
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
        return false;
    }
}
