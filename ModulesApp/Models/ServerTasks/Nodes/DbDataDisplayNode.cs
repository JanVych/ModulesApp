using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbDataDisplayNode : DbTaskNode
{
    public DbDataDisplayNode(TaskNode node) : base(node){}
    public DbDataDisplayNode(){}

    public override void Process(ContextService context)
    {
        if (InputType == NodeInputType.Double)
        {
            var triggerInputValue = GetInputValue(context, PortPositionAlignment.Top, "trigger");
            if (triggerInputValue.Type == NodeValueType.Invalid)
            {
                Value = triggerInputValue;
                return;
            }

            var trigger = DataConvertor.ToBool(triggerInputValue.GetValue());
            if (!trigger)
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, trigger input was false");
                return;
            }
            Value = GetInputValue(context, PortPositionAlignment.Bottom, "data");
        }
        else
        {
            Value = GetInputValue(context, PortPositionAlignment.Center, "data");
        }

        if (Value.Type != NodeValueType.Invalid)
        {
            context.SendToDashboardEntity(LongVal1, "Value", Value.GetValue());
        }
    }
}
