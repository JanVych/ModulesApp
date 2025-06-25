using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ValueNode : TaskNode
{
    public ValueNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.Value;
        LongVal1 = (long)NodeValueType.String;
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.String));
    }

    public ValueNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: (NodeValueType)LongVal1));
    }
}