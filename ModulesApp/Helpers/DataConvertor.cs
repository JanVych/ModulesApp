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
                else if(targetType == typeof(int))
                {
                    result.Add((T)(object)ToInt32(element));
                }
                else if (targetType == typeof(long))
                {
                    result.Add((T)(object)ToInt64(element));
                }
                else if (targetType == typeof(decimal))
                {
                    result.Add((T)(object)ToDecimal(element));
                }
                else if (targetType == typeof(DateTime))
                {
                    result.Add((T)(object)ToDateTime(element));
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

    public static int ToInt32(object? value)
    {
        if (value is int intValue)
        {
            return intValue;
        }
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.GetInt32();
            }
            else if (json.ValueKind == JsonValueKind.String && int.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
        }
        if (value is string str && int.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        return default;
    }

    public static decimal ToDecimal(object? value)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue;
        }
        if (value is int intValue)
        {
            return (decimal)intValue;
        }
        if (value is double doubleValue)
        {
            return (decimal)doubleValue;
        }
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.GetDecimal();
            }
            else if (json.ValueKind == JsonValueKind.String && decimal.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
        }
        if (value is string str && decimal.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        return default;
    }

    public static long ToInt64(object? value)
    {
        if (value is long longValue)
        {
            return longValue;
        }
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.GetInt64();
            }
            else if (json.ValueKind == JsonValueKind.String && long.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
        }
        if (value is string str && long.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        return default;
    }

    public static DateTime ToDateTime(object? value)
    {
        if (value is DateTime dateTimeValue)
        {
            return dateTimeValue;
        }
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.String && DateTime.TryParse(json.GetString(), out var parsed))
            {
                return parsed;
            }
            else if (json.ValueKind == JsonValueKind.Number && DateTime.TryParse(json.GetDouble().ToString(), out parsed))
            {
                return parsed;
            }
        }
        if (value is string str && DateTime.TryParse(str, out var parsedValue))
        {
            return parsedValue;
        }
        return default;
    }

    public static bool ToBool(object? value)
    {
        if (value is bool boolValue)
        {
            return boolValue;
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

    public static double[] ToDoubleArray(object? value)
    {
        if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
        {
            try
            {
                return json.EnumerateArray().Select(e => e.GetDouble()).ToArray();
            }
            catch
            {
                return [];
            }
        }

        if (value is double[] array)
        {
            return array;
        }

        if (value is List<double> list)
        {
            return list.ToArray();
        }

        if (value is IEnumerable<double> enumerable)
        {
            return enumerable.ToArray();
        }

        return [];
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
