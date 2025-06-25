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
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.String));
    }

    public DateTimeNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: GetPortType((NodeDateTimeOutputType)LongVal1)));
    }

    public NodeValueType GetPortType(NodeDateTimeOutputType type)
    {
        return type switch
        {
            NodeDateTimeOutputType.DateTimeString => NodeValueType.String,
            NodeDateTimeOutputType.DateString => NodeValueType.String,
            NodeDateTimeOutputType.TimeString => NodeValueType.String,
            NodeDateTimeOutputType.Year => NodeValueType.Number,
            NodeDateTimeOutputType.Month => NodeValueType.Number,
            NodeDateTimeOutputType.Day => NodeValueType.Number,
            NodeDateTimeOutputType.Hour => NodeValueType.Number,
            NodeDateTimeOutputType.Minute => NodeValueType.Number,
            NodeDateTimeOutputType.Second => NodeValueType.Number,
            NodeDateTimeOutputType.DayOfWeek => NodeValueType.Number,
            NodeDateTimeOutputType.DayOfYear => NodeValueType.Number,
            _ => NodeValueType.Number,
        };
    }
}
