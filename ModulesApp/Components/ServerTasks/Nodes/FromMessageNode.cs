using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class FromMessageNode : TaskNode
{
    public FromMessageNode(ContextService context, Point? position = null) : base(context, position) 
    {
        Type = NodeType.FromMessage;
        LongVal1 = (long)NodeValueType.Any;
        AddPort(new TaskPort(this, false, 0, data: true));
    }

    public FromMessageNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, 0, data: true));
    }
}
