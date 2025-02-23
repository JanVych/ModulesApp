using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ValueNode : TaskNode
{
    public ValueNode(IServerContext context, Point? position = null) : base(context, position)
    {
        StringVal2 = "Number";
        Type = NodeType.Value;
        AddPort(new TaskPort(this, false, 0, data: true));
    }

    public ValueNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, 0, data: true));
    }
}
