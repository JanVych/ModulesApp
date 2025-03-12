using static MudBlazor.Colors;

namespace ModulesApp.Models.ServerTasks;

public enum NodeValueType
{
    Waiting,
    String,
    Number,
    Boolean,
    Array,
    Json,
    Invalid
}

public abstract class NodeValue
{
    public abstract NodeValueType Type { get; }

    public class Waiting() : NodeValue
    {
        public override NodeValueType Type => NodeValueType.Waiting;
    }

    public class StringValue(string Value) : NodeValue
    {
        public string Value { get; init; } = Value;
        public override string? ToString() => Value;
        public override NodeValueType Type => NodeValueType.String;
    }

    public class NumberValue(double Value) : NodeValue
    {
        public double Value { get; init; } = Value;
        public override string? ToString() => Value.ToString();
        public override NodeValueType Type => NodeValueType.Number;
    }

    public class BooleanValue(bool Value) : NodeValue
    {
        public bool Value { get; init; } = Value;
        public override string? ToString() => Value.ToString();
        public override NodeValueType Type => NodeValueType.Boolean;
    }

    public class ArrayValue(List<NodeValue> Value) : NodeValue
    {
        public List<NodeValue> Value { get; init; } = Value;
        public override string? ToString() => string.Join(", ", Value.Select(v => v.ToString()));
        public List<string?> ToStringList() => Value.Select(v => v.ToString()).ToList();
        public override NodeValueType Type => NodeValueType.Array;
    }

    public class JsonValue(Dictionary<string, object> Value) : NodeValue
    {
        public Dictionary<string, object> Value { get; init; } = Value;
        public override string? ToString() => Value.ToString() ?? string.Empty;
        public override NodeValueType Type => NodeValueType.Json;
    }

    public class InvalidValue(string Reason) : NodeValue
    {
        public string Value { get; init; } = Reason;
        public override string? ToString() => Value;
        public override NodeValueType Type => NodeValueType.Invalid;
    }
}
