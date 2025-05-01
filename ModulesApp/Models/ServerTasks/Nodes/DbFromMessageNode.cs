using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;
using System.Text.Json;
using static MudBlazor.CategoryTypes;

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
        //Value = TargetLinks.FirstOrDefault()?.GetValue(context)
        //        ?? new NodeValue.InvalidValue($"node: {Order}, had no input");

        if (Task == null)
        {
            Value = new NodeValue.InvalidValue($"Module not found, in node: {Order}");
            return;
        }

        JsonElement? value = null;
        if (Task.ModuleId is long moduleId)
        {
            value = context.GetMessageFromModule(moduleId, StringVal1);
        }

        else if(Task.BackgroundServiceId is long backgroundServiceId)
        {
            value = context.GetMessageFromService(backgroundServiceId, StringVal1);
        }

        else if (Task.DashboardEntityId is long dashboardEntityId)
        {
            value = context.GetMessageFromDashBoardEntity(dashboardEntityId, StringVal1);
        }


        if (value is null)
        {
            Value = new NodeValue.InvalidValue($"No such key:{StringVal1} in message, from module: {Task.ModuleId}, in node: {Order}");
            return;
        }

        Value = ConvertJsonElement((JsonElement)value);

        if (!IsValidType(Value, StringVal2))
        {
            if(StringVal2 == "number")
            {
                Value = new NodeValue.NumberValue(DataConvertor.ToDouble(value));
            }
            else
            {
                Value = new NodeValue.InvalidValue($"Value is not {StringVal2}, from module: {Task.ModuleId}, in node: {Order}");
            }
        }
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

    private static bool IsValidType(NodeValue value, string expectedType)
    {
        return expectedType switch
        {
            "any" => true,
            "number" => value.Type == NodeValueType.Number,
            "string" => value.Type == NodeValueType.String,
            "boolean" => value.Type == NodeValueType.Boolean,
            "array" => value.Type == NodeValueType.Array,
            _ => false
        };
    }
}
