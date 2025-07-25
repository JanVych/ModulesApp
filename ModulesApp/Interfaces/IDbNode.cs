namespace ModulesApp.Interfaces;

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
    ArrayRemoveAt
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

public enum PortPositionAlignment
{
    Top,
    Center,
    Bottom,
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

    public static string ToLongString(this NodeConditionType type, char space='-')
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
            _ => type.ToString()
        };
    }
}

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
    public double DoubleVal2 { get; set; }
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public long LongVal3 { get; set; }
    public bool BoolVal1 { get; set; }
    public bool BoolVal2 { get; set; }

    public int Order { get; set; }

    public double PositionX { get; }
    public double PositionY { get; }
}

