using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArithmeticOperationNode : TaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;

    public ArithmeticOperationNode(ContextService context, NodeArithmeticOperationType operationType, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ArithmeticOperation;
        SubType = (int)operationType;
        AddPorts(NodeInputType.Single);
    }

    public ArithmeticOperationNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(dbNode.InputType);
    }

    private void AddPorts(NodeInputType input)
    {
        AddInputPorts(input);
        //Output data port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }

    public void AddInputPorts(NodeInputType input)
    {
        InputType = input;
        RemoveAllInputPorts();
        if (input == NodeInputType.Double)
        {
            //Input right operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Start, data: true));
            //Input left operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.End, data: true));
        }
        else
        {
            //Input right operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, data: true));
        }
    }
}
