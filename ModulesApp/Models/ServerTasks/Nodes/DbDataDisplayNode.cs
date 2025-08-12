using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Models.Dasboards;
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
            var entity = context._dashboardService.GetEntity(LongVal1);
            string key = entity?.Type switch
            {
                DashboardEntityType.KeyValue => "Value",
                DashboardEntityType.DataList => "Column_2",
                DashboardEntityType.ValueSetter => "CurrentValue",
                DashboardEntityType.Switch => "Value",
                _ => "Value"
            };
            context.SendToDashboardEntity(LongVal1, key, Value.GetValue());
        }
    }
}
