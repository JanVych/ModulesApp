using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ArithmeticOperationNode : TaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;

    public ArithmeticOperationNode(IServerContext context, NodeArithmeticOperationType operationType, Point? position = null, NodeInputType input = NodeInputType.Single)
        : base(context, position)
    {
        Type = NodeType.ArithmeticOperation;
        SubType = (int)operationType;
        InputType = input;
        AddPorts(InputType);
    }

    public ArithmeticOperationNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(dbNode.InputType);
    }

    private void AddPorts(NodeInputType input)
    {
        //Output data port
        AddPort(new TaskPort(this, false, 0, data: true));

        
        if (input == NodeInputType.Double)
        {
            //Input right operation port
            AddPort(new TaskPort(this, true, 1, data: true));
            //Input left operation port
            AddPort(new TaskPort(this, true, 2, data: true));
        }
        else
        {
            //Input right operation port
            AddPort(new TaskPort(this, true, 0, data: true));
        }
    }
}
