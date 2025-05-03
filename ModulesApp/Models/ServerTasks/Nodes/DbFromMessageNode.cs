using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbFromMessageNode : DbTaskNode
{
    public DbFromMessageNode(TaskNode node) : base(node)
    {
    }

    public DbFromMessageNode()
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
        JsonElement? value = null;
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
        else if (Task.TriggerSourceType == TargetType.DashboardEntity && Task.DashboardEntityId is long dashboardEntityId)
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

        if (!IsValidType(jValue, (NodeValueType)LongVal1))
        {
            Value = new NodeValue.InvalidValue($"Value is not {(NodeValueType)LongVal1}, from module: {Task.ModuleId}, in node: {Order}");
            return;
        }
        Value = ConvertJsonElement((JsonElement)value);
    }

    private NodeValue ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => new NodeValue.StringValue(element.GetString() ?? string.Empty),
            JsonValueKind.Number => new NodeValue.NumberValue(element.GetDouble()),
            JsonValueKind.True => new NodeValue.BooleanValue(true),
            JsonValueKind.False => new NodeValue.BooleanValue(false),
            JsonValueKind.Array => new NodeValue.ArrayValue(element.EnumerateArray().Select(ConvertJsonElement).ToList()),
            _ => new NodeValue.InvalidValue($"Invalid value type: {element.ValueKind}, from module: {Task.ModuleId}, in node: {Order}"),
        };
    }

    private static bool IsValidType(JsonElement element, NodeValueType type)
    {
        return type switch
        {
            NodeValueType.Any => true,
            NodeValueType.String => element.ValueKind == JsonValueKind.String,
            NodeValueType.Number => element.ValueKind == JsonValueKind.Number,
            NodeValueType.Boolean => element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False,
            NodeValueType.Array => element.ValueKind == JsonValueKind.Array,
            _ => false
        };
    }
}
