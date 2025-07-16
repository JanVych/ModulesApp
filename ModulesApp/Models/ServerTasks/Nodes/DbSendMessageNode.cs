using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbSendMessageNode : DbTaskNode
{
    public DbSendMessageNode(TaskNode node) : base(node){}
    public DbSendMessageNode(){}

    public override void Process(ContextService context)
    {
        if(string.IsNullOrEmpty(StringVal1))
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, key cant be empty!");
            return;
        }

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

        if (Value.Type == NodeValueType.Invalid)
        {
            return;
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
            Value = new NodeValue.InvalidValue($"In node: {Order}, invalid type: {(TargetType)LongVal2}!");
        }
    }
}