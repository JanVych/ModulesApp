using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public abstract class TaskNode : NodeModel, IDbNode
{
    public NodeType Type { get; set; }
    public int SubType { get; set; }
    public NodeInputType InputType { get; set; } = NodeInputType.None;

    public readonly IServerContext _context;

    public double PositionX => Position.X;
    public double PositionY => Position.Y;

    public string StringVal1 { get; set; } = string.Empty;
    public string StringVal2 { get; set; } = string.Empty;
    public string StringVal3 { get; set; } = string.Empty;
    public double DoubleVal1 { get; set; }
    public bool BoolVal1 { get; set; }
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public long LongVal3 { get; set; }

    public TaskNode(IServerContext context, Point? position = null) : base(position)
    {
        _context = context;
    }

    public TaskNode(IServerContext context, DbTaskNode dbNode) :base(new Point(dbNode.PositionX, dbNode.PositionY))
    {
        _context = context;
        Order = dbNode.Order;
        Type = dbNode.Type;
        SubType = dbNode.SubType;
        InputType = dbNode.InputType;
        
        StringVal1 = dbNode.StringVal1;
        StringVal2 = dbNode.StringVal2;
        StringVal3 = dbNode.StringVal3;
        DoubleVal1 = dbNode.DoubleVal1;
        LongVal1 = dbNode.LongVal1;
        LongVal2 = dbNode.LongVal2;
        LongVal3 = dbNode.LongVal3;
        BoolVal1 = dbNode.BoolVal1;
    }
    public void RemoveAllInputPorts()
    {
        foreach (var port in Ports.ToList())
        {
            if (port is TaskPort taskPort && taskPort.Input)
            {
                taskPort.RemoveAllLinks();
                RemovePort(port);
            }
        }
    }
}
