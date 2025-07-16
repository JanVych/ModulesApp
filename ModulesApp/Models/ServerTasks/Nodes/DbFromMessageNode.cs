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
            Value = new NodeValue.InvalidValue($"In node: {Order}, source not found!");
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
            Value = new NodeValue.InvalidValue($"In node: {Order}, invalid source type: {Task.TriggerSourceType}!");
            return;
        }

        if (value is not JsonElement jValue)
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, no such key:{StringVal1} in {Task.TriggerSourceType}!");
            return;
        }

        if (!NodeValue.IsValidType(jValue, (NodeValueType)LongVal1))
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, value is not {(NodeValueType)LongVal1}, but: {jValue.ValueKind}!");
            return;
        }
        Value = NodeValue.CreateFromJsonElement(jValue, this);
    }
}
