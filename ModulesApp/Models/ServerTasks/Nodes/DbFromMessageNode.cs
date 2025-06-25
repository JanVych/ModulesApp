using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbFromMessageNode : DbTaskNode
{
    public DbFromMessageNode(TaskNode node) : base(node){}
    public DbFromMessageNode(){}

    public override void Process(ContextService context)
    {
        JsonElement? value;
        if (Task == null)
        {
            Value = new NodeValue.InvalidValue($"Source not found, in node: {Order}");
            return;
        }

        if(Task.TriggerSourceType == TargetType.Module && Task.ModuleId is long moduleId)
        {
            value = context.GetMessageFromModule(moduleId, StringVal1);
        }
        else if (Task.TriggerSourceType == TargetType.Service && Task.BackgroundServiceId is long backgroundServiceId)
        {
            value = context.GetMessageFromService(backgroundServiceId, StringVal1);
        }
        else if (Task.TriggerSourceType == TargetType.Dashboard && Task.DashboardEntityId is long dashboardEntityId)
        {
            value = context.GetMessageFromDashBoardEntity(dashboardEntityId, StringVal1);
        }
        else
        {
            Value = new NodeValue.InvalidValue($"Invalid source type: {Task.TriggerSourceType}, in node: {Order}");
            return;
        }

        if (value is not JsonElement jValue)
        {
            Value = new NodeValue.InvalidValue($"No such key:{StringVal1} in message, from {Task.TriggerSourceType}, in node: {Order}");
            return;
        }

        if (!NodeValue.IsValidType(jValue, (NodeValueType)LongVal1))
        {
            Value = new NodeValue.InvalidValue($"Value is not {(NodeValueType)LongVal1}, but: {jValue.ValueKind}, from {Task.TriggerSourceType}, in node: {Order}");
            return;
        }
        Value = NodeValue.CreateFromJsonElement(jValue, this);
    }
}
