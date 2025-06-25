using System.Text.Json;

namespace ModulesApp.Models.ServerTasks;

public enum NodeValueType
{
    Waiting,
    String,
    Number,
    Boolean,
    Array,
    Object,
    Invalid,
    Any,
    NoData
}

public abstract class NodeValue
{
    public abstract NodeValueType Type { get; }
    public abstract object? GetValue();

    public class Waiting() : NodeValue
    {
        public override NodeValueType Type => NodeValueType.Waiting;
        public override object? GetValue() => null;
    }

    public class InvalidValue(string Reason) : NodeValue
    {
        public string Value { get; init; } = Reason;
        public override object? GetValue() => null;
        public override string? ToString() => Value;
        public override NodeValueType Type => NodeValueType.Invalid;
    }

    public class StringValue(string Value) : NodeValue
    {
        public string Value { get; init; } = Value;
        public override object? GetValue() => Value;
        public override string? ToString() => Value;
        public override NodeValueType Type => NodeValueType.String;
    }

    public class NumberValue(double Value) : NodeValue
    {
        public double Value { get; init; } = Value;
        public override object? GetValue() => Value;
        public override string? ToString() => Value.ToString();
        public override NodeValueType Type => NodeValueType.Number;
    }

    public class BooleanValue(bool Value) : NodeValue
    {
        public bool Value { get; init; } = Value;
        public override object? GetValue() => Value;
        public override string? ToString() => Value.ToString();
        public override NodeValueType Type => NodeValueType.Boolean;
    }

    public class ArrayValue(List<NodeValue> value) : NodeValue
    {
        public IReadOnlyList<NodeValue> Value { get; init; } = value;
        public override List<object?> GetValue() => Value.Select(v => v.GetValue()).ToList();
        public override string? ToString() => string.Join(", ", Value.Select(v => v.ToString()));
        public override NodeValueType Type => NodeValueType.Array;
        public List<NodeValue> GetValueClone()
        {
            List<NodeValue> clone = [];
            foreach (var v in Value)
            {
                if (v is ArrayValue array)
                {
                    clone.Add(new ArrayValue(array.GetValueClone()));
                }
                //else if (v is ObjectValue obj)
                //{
                //    clone.Add(new ObjectValue(obj.Value));
                //}
                else if (v is StringValue str)
                {
                    clone.Add(new StringValue(str.Value));
                }
                else if (v is NumberValue num)
                {
                    clone.Add(new NumberValue(num.Value));
                }
                else if (v is BooleanValue boolean)
                {
                    clone.Add(new BooleanValue(boolean.Value));
                }
            }
            return clone;
        }
    }

    // TODO
    //public class ObjectValue(Dictionary<string, object?> Value) : NodeValue
    //{
    //    public Dictionary<string, object?> Value { get; init; } = Value;
    //    public override Dictionary<string, object?> GetValue() => Value;
    //    public override string? ToString() => Value.ToString() ?? string.Empty;
    //    public override NodeValueType Type => NodeValueType.Object;
    //}

    public static NodeValue CreateFromJsonElement(JsonElement element, DbTaskNode node)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => new StringValue(element.GetString() ?? string.Empty),
            JsonValueKind.Number => new NumberValue(element.GetDouble()),
            JsonValueKind.True => new BooleanValue(true),
            JsonValueKind.False => new BooleanValue(false),
            JsonValueKind.Array => new ArrayValue(element.EnumerateArray().Select(e => CreateFromJsonElement(e, node)).ToList()),
            _ => new InvalidValue($"Invalid value type: {element.ValueKind}, in node: {node.Order}"),
        };
    }
    public static bool IsValidType(JsonElement element, NodeValueType type)
    {
        return type switch
        {
            NodeValueType.Any => true,
            NodeValueType.String => element.ValueKind == JsonValueKind.String,
            NodeValueType.Number => element.ValueKind == JsonValueKind.Number,
            NodeValueType.Boolean => element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False,
            NodeValueType.Array => element.ValueKind == JsonValueKind.Array,
            _ => false
        };
    }

    public static bool GetBooleanValue(NodeValue value)
    {
        return value.Type switch
        {
            NodeValueType.Boolean => ((BooleanValue)value).Value,
            NodeValueType.String => bool.TryParse(((StringValue)value).Value, out var result) && result,
            NodeValueType.Number => ((NumberValue)value).Value != 0,
            NodeValueType.Array => ((ArrayValue)value).Value.Count > 0,
            _ => false
        };
    }
}
