using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbSendMessageNode : DbTaskNode
{
    public DbSendMessageNode(TaskNode node) : base(node){}
    public DbSendMessageNode(){}

    private NodeValue GetInputValue(ContextService context)
    {
        DbTaskLink? link;
        if (InputType == NodeInputType.Single)
        {
            link = TargetLinks.FirstOrDefault(l => l.TargetInput);
        }
        else
        {
            link = TargetLinks.FirstOrDefault(l => l.TargetInput && l.TargetPositionAlignment == PortPositionAlignment.Bottom);
        }
        if (link == null)
        {
            return new NodeValue.InvalidValue($"node: {Order}, no data input");
        }
        return link.GetValue(context);
    }

    private NodeValue GetInputTriggerValue(ContextService context)
    {
        return TargetLinks?.FirstOrDefault(l => l.TargetInput && l.TargetPositionAlignment == PortPositionAlignment.Top)
            ?.GetValue(context) ?? 
            new NodeValue.InvalidValue($"node: {Order}, no trigger input");
    }

    public override void Process(ContextService context)
    {
        Value = GetInputValue(context);
        if (Value.Type == NodeValueType.Invalid)
        {
            return;
        }

        if(InputType == NodeInputType.Double)
        {
            var triggerValue = GetInputTriggerValue(context);
            if (triggerValue.Type == NodeValueType.Invalid)
            {
                Value = triggerValue;
                return;
            }
            var result = NodeValue.GetBooleanValue(triggerValue);
            if (!result)
            {
                Value = new NodeValue.InvalidValue($"node: {Order}, trigger input was false");
                return;
            }
        }

        if (LongVal2 == (long)TargetType.Module)
        {
            context.SendToModule(LongVal1, StringVal1, Value.GetValue());
        }
        else if (LongVal2 == (long)TargetType.Service)
        {
            context.SendToBackgroundService(LongVal1, StringVal1, Value.GetValue());
        }
        else if (LongVal2 == (long)TargetType.Dashboard)
        {
            context.SendToDashboardEntity(LongVal1, StringVal1, Value.GetValue());
        }
        else
        {
            Value = new NodeValue.InvalidValue($"Invalid type: {(TargetType)LongVal2}, in node: {Order}");
        }
    }
}