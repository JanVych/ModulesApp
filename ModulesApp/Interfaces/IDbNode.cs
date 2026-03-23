using ModulesApp.Types;

namespace ModulesApp.Interfaces;

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

