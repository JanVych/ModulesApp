using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbDataDisplayNode : DbTaskNode
{
    public DbDataDisplayNode(TaskNode node) : base(node){}
    public DbDataDisplayNode(){}

    private NodeValue GetInputValue(ContextService context)
    {
        DbTaskLink? link;
        if (InputType == NodeInputType.Single)
        {
            link = TargetLinks.FirstOrDefault();
        }
        else
        {
            link = TargetLinks.FirstOrDefault(l => l.TargetPositionAlignment == PortPositionAlignment.Bottom);
        }
        if (link == null)
        {
            return new NodeValue.InvalidValue($"node: {Order}, no data input");
        }
        return link.GetValue(context);
    }

    private NodeValue GetInputTriggerValue(ContextService context)
    {
        return TargetLinks?.FirstOrDefault(l => l.TargetPositionAlignment == PortPositionAlignment.Top)
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

        if (InputType == NodeInputType.Double)
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
        context.SendToDashboardEntity(LongVal1, "Value", Value.GetValue());
    }
}
