using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;
using ModulesApp.Types;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArrayOperationNode :TaskNode
{
    public NodeArrayOperationType OperationType => (NodeArrayOperationType)SubType;

    public ArrayOperationNode(ContextService context, NodeArrayOperationType operationType, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ArrayOperation;
        SubType = (int)operationType;
        if (operationType == NodeArrayOperationType.ArrayAppend || operationType == NodeArrayOperationType.ArrayMerge)
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
        InputType = input;
        RemoveAllInputPorts();

        if (input == NodeInputType.Double)
        {
            var secondType = SubType switch
            {
                (int)NodeArrayOperationType.ArrayAppend => NodeValueType.Any,
                (int)NodeArrayOperationType.ArrayMerge => NodeValueType.Array,
                _ => NodeValueType.Number
            };

            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.Array));
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: secondType));
        }
        else if (input == NodeInputType.Single)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Array));
        }
    }
}
