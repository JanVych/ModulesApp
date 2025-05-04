using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ConditionNode : TaskNode
{
    public NodeConditionType ConditionType => (NodeConditionType)SubType;
    public ConditionNode(ContextService context, NodeConditionType condition, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.Condition;
        SubType = (int)condition;
        AddPorts(NodeInputType.Single);

    }

    public ConditionNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(dbNode.InputType);
    }

    private void AddPorts(NodeInputType input)
    {
        AddInputPorts(input);
        //Output data port True
        AddPort(new TaskPort(this, false, PortPositionAlignment.Start, data: true));
        //Output data port False
        AddPort(new TaskPort(this, false, PortPositionAlignment.End, data: true));
    }

    public void AddInputPorts(NodeInputType input)
    {
        InputType = input;
        RemoveAllInputPorts();
        if (input == NodeInputType.Double)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Start, data: true));
            AddPort(new TaskPort(this, true, PortPositionAlignment.End, data: false));
        }
        else
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, data: true));
        }
    }
}
