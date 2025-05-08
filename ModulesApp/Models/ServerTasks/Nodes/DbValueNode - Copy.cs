using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbDateTimeNode : DbTaskNode
{
    public DbDateTimeNode(TaskNode node) : base(node)
    {
    }

    public DbDateTimeNode()
    {
    }

    public override NodeValue GetValue(DbTaskLink dbLink, ContextService context)
    {
        if (Value.Type == NodeValueType.Waiting)
        {
            Process(context);
        }
        return Value;
    }

    public override void Process(ContextService context)
    {
        var dateTime = DateTime.Now;
        NodeDateTimeOutputType outputType = (NodeDateTimeOutputType)LongVal1;

        Value = outputType switch
        {
            NodeDateTimeOutputType.DateTimeString => new NodeValue.StringValue(dateTime.ToString()),
            NodeDateTimeOutputType.DateString => new NodeValue.StringValue(dateTime.ToShortDateString()),
            NodeDateTimeOutputType.TimeString => new NodeValue.StringValue(dateTime.ToShortTimeString()),
            NodeDateTimeOutputType.Year => new NodeValue.NumberValue(dateTime.Year),
            NodeDateTimeOutputType.Month => new NodeValue.NumberValue(dateTime.Month),
            NodeDateTimeOutputType.Day => new NodeValue.NumberValue(dateTime.Day),
            NodeDateTimeOutputType.Hour => new NodeValue.NumberValue(dateTime.Hour),
            NodeDateTimeOutputType.Minute => new NodeValue.NumberValue(dateTime.Minute),
            NodeDateTimeOutputType.Second => new NodeValue.NumberValue(dateTime.Second),
            NodeDateTimeOutputType.DayOfWeek => new NodeValue.NumberValue((double)dateTime.DayOfWeek),
            NodeDateTimeOutputType.DayOfYear => new NodeValue.NumberValue(dateTime.DayOfYear),
            _ => new NodeValue.InvalidValue($"Not supported type: {outputType}, node: {Order}")
        };
    }
}
