namespace ModulesApp.Interfaces;

public enum NodeType
{
    None,
    Condition,
    FromMessage,
    DataDisplay,
    Value,
    SendMessage,
    ArrayOperation,
    ArithmeticOperation,
}

public enum NodeConditionType
{
    None,
    Equal,
    NotEqual,
    Greater,
    Less,
    GreaterOrEqual,
    LessOrEqual
}

public enum NodeArrayOperationType
{
    ArraySlice,
    ArrayRemoveAt
}

public enum NodeArithmeticOperationType
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public static class NodeExtensions
{
    public static string ToShortString(this NodeConditionType type)
    {
        return type switch
        {
            NodeConditionType.None => "None",
            NodeConditionType.Equal => "==",
            NodeConditionType.NotEqual => "!=",
            NodeConditionType.Greater => ">",
            NodeConditionType.Less => "<",
            NodeConditionType.GreaterOrEqual => ">=",
            NodeConditionType.LessOrEqual => "<=",
            _ => type.ToString()
        };
    }

    public static string ToLongString(this NodeConditionType type)
    {
        return type switch
        {
            NodeConditionType.None => "None",
            NodeConditionType.Equal => "Equal",
            NodeConditionType.NotEqual => "Not Equal",
            NodeConditionType.Greater => "Greater",
            NodeConditionType.Less => "Less",
            NodeConditionType.GreaterOrEqual => "Greater Or Equal",
            NodeConditionType.LessOrEqual => "Less Or Equal",
            _ => type.ToString()
        };
    }

    public static string ToLongString(this NodeArrayOperationType type)
    {
        return type switch
        {
            NodeArrayOperationType.ArraySlice => "Array Slice",
            NodeArrayOperationType.ArrayRemoveAt => "Array Remove At",
            _ => type.ToString()
        };
    }
}

//public enum NodeOperationType
//{
//    None,
//    Add,
//    Subtract,
//    Multiply,
//    Divide
//}

public enum NodeInputType
{
    None,
    Single,
    Double,
}

public interface IDbNode
{
    public NodeType Type { get; set; }
    public int SubType { get; set; }
    public NodeInputType InputType { get; set; }


    public string StringVal1 { get; set; }
    public string StringVal2 { get; set; }
    public string StringVal3 { get; set; }
    public double DoubleVal1 { get; set; }
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public bool BoolVal1 { get; set; }

    public int Order { get; set; }

    public double PositionX { get; }
    public double PositionY { get; }
}

