using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArithmeticSaturationNode : TaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;

    public ArithmeticSaturationNode(ContextService context, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ArithmeticSaturation;
        BoolVal1 = true;
        BoolVal2 = true;
        AddPorts();
    }

    public ArithmeticSaturationNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        //Input port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Number));
        //Output port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.Number));
    }
}
