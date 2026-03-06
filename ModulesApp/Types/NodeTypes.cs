namespace ModulesApp.Types;

public enum NodeType
{
    Condition,
    FromMessage,
    DataDisplay,
    Value,
    SendMessage,
    ArrayOperation,
    ArithmeticOperation,
    ConvertTo,
    DateTime,
    FromAny,
    BooleanOperation,
    ArithmeticSaturation,
    Branch,
    GlobalValue,
}

public enum NodeConditionType
{
    Equal,
    NotEqual,
    Greater,
    Less,
    GreaterOrEqual,
    LessOrEqual,
}

public enum NodeArrayOperationType
{
    ArraySlice,
    ArrayRemoveAt,
    ArrayAppend,
    ArrayCreate,
    ArrayMerge
}

public enum NodeArithmeticOperationType
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public enum NodeDateTimeOutputType
{
    DateTimeString,
    DateString,
    TimeString,
    Year,
    Month,
    Day,
    Hour,
    Minute,
    Second,
    DayOfWeek,
    DayOfYear
}

public enum NodeBooleanOperationType
{
    And,
    Or,
    Not,
    Xor,
    Nand,
    Nor,
    Xnor,
}

public enum NodeModeType
{
    Get,
    Set,
}

public enum PortPositionAlignment
{
    Top,
    Center,
    Bottom,
}

public enum NodeInputType
{
    None,
    Single,
    Double,
}
public static class NodeExtensions
{
    public static string ToShortString(this NodeConditionType type)
    {
        return type switch
        {
            NodeConditionType.Equal => "==",
            NodeConditionType.NotEqual => "!=",
            NodeConditionType.Greater => ">",
            NodeConditionType.Less => "<",
            NodeConditionType.GreaterOrEqual => ">=",
            NodeConditionType.LessOrEqual => "<=",
            _ => type.ToString()
        };
    }

    public static string ToLongString(this NodeConditionType type, char space = '-')
    {
        return type switch
        {
            NodeConditionType.Equal => "Equal",
            NodeConditionType.NotEqual => $"Not{space}Equal",
            NodeConditionType.Greater => "Greater",
            NodeConditionType.Less => "Less",
            NodeConditionType.GreaterOrEqual => $"Greater{space}Or{space}Equal",
            NodeConditionType.LessOrEqual => $"Less{space}Or{space}Equal",
            _ => type.ToString()
        };
    }

    public static string ToLongString(this NodeArrayOperationType type, char space = '-')
    {
        return type switch
        {
            NodeArrayOperationType.ArraySlice => "Slice",
            NodeArrayOperationType.ArrayRemoveAt => $"Remove{space}At",
            NodeArrayOperationType.ArrayAppend => "Append",
            NodeArrayOperationType.ArrayCreate => "Create",
            NodeArrayOperationType.ArrayMerge => "Merge",
            _ => type.ToString()
        };
    }

    public static string ToLongString(this NodeType type, char space = '-')
    {
        return type switch
        {
            NodeType.Condition => "Condition",
            NodeType.FromMessage => $"From{space}Message",
            NodeType.DataDisplay => $"Data{space}Display",
            NodeType.Value => "Value",
            NodeType.SendMessage => $"Send{space}Message",
            NodeType.ArrayOperation => "Array",
            NodeType.ArithmeticOperation => "Arithmetic",
            NodeType.ConvertTo => "To",
            NodeType.DateTime => $"Date{space}Time",
            NodeType.FromAny => $"From{space}Any",
            NodeType.BooleanOperation => "Boolean",
            NodeType.ArithmeticSaturation => "Saturation",
            NodeType.GlobalValue => "Global",
            _ => type.ToString()
        };
    }
}

