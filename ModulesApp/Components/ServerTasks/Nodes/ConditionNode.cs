using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ConditionNode : TaskNode
{
    public NodeConditionType ConditionType => (NodeConditionType)SubType;
    public ConditionNode(IServerContext context, NodeConditionType condition, Point? position = null, NodeInputType input = NodeInputType.Single)
        : base(context, position)
    {
        Type = NodeType.Condition;
        SubType = (int)condition;
        InputType = input;
        AddPorts(input);

    }

    public ConditionNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(dbNode.InputType);
    }

    private void AddPorts(NodeInputType input)
    {
        //Output data port True
        AddPort(new TaskPort(this, false, 1, data: true));
        //Output data port False
        AddPort(new TaskPort(this, false, 2, data: true));

        //Input secondary port
        if (input == NodeInputType.Double)
        {
            AddPort(new TaskPort(this, true, 1, data: true));
            AddPort(new TaskPort(this, true, 2, data: false));
        }
        else
        {
            AddPort(new TaskPort(this, true, 0, data: true));
        }
    }
}
