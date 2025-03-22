using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArrayOperationNode :TaskNode
{
    public NodeArrayOperationType OperationType => (NodeArrayOperationType)SubType;
    public ArrayOperationNode(IServerContext context, NodeArrayOperationType condition, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ArrayOperation;
        SubType = (int)condition;
        AddPorts();
    }

    public ArrayOperationNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        //Output data port
        AddPort(new TaskPort(this, false, 0, data: true));

        //Input data port
        AddPort(new TaskPort(this, true, 0, data: true));
    }
}
