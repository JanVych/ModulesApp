using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArrayOperationNode :TaskNode
{
    public NodeArrayOperationType OperationType => (NodeArrayOperationType)SubType;

    public ArrayOperationNode(ContextService context, NodeArrayOperationType operationType, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ArrayOperation;
        SubType = (int)operationType;
        AddPorts();
    }

    public ArrayOperationNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        //Output data port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));

        //Input data port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Center, data: true));
    }
}
