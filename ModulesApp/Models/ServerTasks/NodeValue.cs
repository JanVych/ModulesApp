namespace ModulesApp.Models.ServerTasks;

public enum NodeValueType
{
    Waiting,
    String,
    Number,
    Boolean,
    Array,
    Object,
    Invalid
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

    public class ArrayValue(List<NodeValue> Value) : NodeValue
    {
        public List<NodeValue> Value { get; init; } = Value;
        public override object? GetValue() => Value.Select(v => v.GetValue()).ToList();
        public override string? ToString() => string.Join(", ", Value.Select(v => v.ToString()));
        public List<string?> ToStringList() => Value.Select(v => v.ToString()).ToList();
        public override NodeValueType Type => NodeValueType.Array;
    }

    public class ObjectValue(Dictionary<string, object> Value) : NodeValue
    {
        public Dictionary<string, object> Value { get; init; } = Value;
        public override object? GetValue() => Value;
        public override string? ToString() => Value.ToString() ?? string.Empty;
        public override NodeValueType Type => NodeValueType.Object;
    }

    public class InvalidValue(string Reason) : NodeValue
    {
        public string Value { get; init; } = Reason;
        public override object? GetValue() => null;
        public override string? ToString() => Value;
        public override NodeValueType Type => NodeValueType.Invalid;
    }
}
