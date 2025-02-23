using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public abstract class TaskNode : NodeModel, IDbNode
{
    public NodeType Type { get; set; } = NodeType.None;
    public int SubType { get; set; }
    public NodeInputType InputType { get; set; } = NodeInputType.None;

    public double PositionX => Position.X;
    public double PositionY => Position.Y;

    public string StringVal1 { get; set; } = string.Empty;
    public string StringVal2 { get; set; } = string.Empty;
    public string StringVal3 { get; set; } = string.Empty;
    public double DoubleVal1 { get; set; }
    public bool BoolVal1 { get; set; }
    public long LongVal1 { get; set; }

    public TaskNode(IServerContext context, Point? position = null) : base(position)
    {
    }

    public TaskNode(IServerContext context, DbTaskNode dbNode) :base(new Point(dbNode.PositionX, dbNode.PositionY))
    {
        Order = dbNode.Order;
        Type = dbNode.Type;
        SubType = dbNode.SubType;
        InputType = dbNode.InputType;

        //ListSelected1 = dbNode.ListSelected1;
        
        StringVal1 = dbNode.StringVal1;
        StringVal2 = dbNode.StringVal2;
        StringVal3 = dbNode.StringVal3;
        DoubleVal1 = dbNode.DoubleVal1;
        LongVal1 = dbNode.LongVal1;
        BoolVal1 = dbNode.BoolVal1;
    }
}
