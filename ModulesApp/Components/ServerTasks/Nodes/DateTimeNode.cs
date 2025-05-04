using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class DateTimeNode : TaskNode
{
    public DateTimeNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.DateTime;
        LongVal1 = (long)NodeDateTimeOutputType.DateTimeString;
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }

    public DateTimeNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }
}
