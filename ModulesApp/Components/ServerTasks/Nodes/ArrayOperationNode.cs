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
        if (operationType == NodeArrayOperationType.ArrayAppend)
        {
            AddPorts(NodeInputType.Double);
        }
        else if(operationType == NodeArrayOperationType.ArrayCreate)
        {
            AddPorts(NodeInputType.None);
        }
        else
        {
            AddPorts(NodeInputType.Single);
        }
    }

    public ArrayOperationNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(dbNode.InputType);
    }

    private void AddPorts(NodeInputType input)
    {
        //Output data port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.Array));

        //Input data port
        AddInputPorts(input);
    }

    public void AddInputPorts(NodeInputType input)
    {
        var secondType = SubType == (int)NodeArrayOperationType.ArrayAppend
            ? NodeValueType.Any
            : NodeValueType.Number;
        InputType = input;
        RemoveAllInputPorts();
        if (input == NodeInputType.Double)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.Array));
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: secondType));
        }
        else if (input == NodeInputType.Single)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Array));
        }
    }
}
