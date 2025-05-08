using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbFromAnyNode : DbTaskNode
{
    public DbFromAnyNode(TaskNode node) : base(node)
    {
    }

    public DbFromAnyNode()
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
        TargetType targetType = (TargetType)LongVal2;
        JsonElement? value;
        if (targetType == TargetType.Module)
        {
            value = context.GetMessageFromModule(LongVal1, StringVal1);
        }
        else if (targetType == TargetType.Service)
        {
            value = context.GetMessageFromService(LongVal1, StringVal1);
        }
        else if (targetType == TargetType.Dashboard)
        {
            value = context.GetMessageFromDashBoardEntity(LongVal1, StringVal1);
        }
        else
        {
            Value = new NodeValue.InvalidValue($"Invalid source type: {targetType}, in node: {Order}");
            return;
        }

        if (value is not JsonElement jValue)
        {
            Value = new NodeValue.InvalidValue($"No such key:{StringVal1} in message, from {targetType}, in node: {Order}");
            return;
        }

        if (!IsValidType(jValue, (NodeValueType)LongVal3))
        {
            Value = new NodeValue.InvalidValue($"Value is not {(NodeValueType)LongVal3}, from module: {Task.ModuleId}, in node: {Order}");
            return;
        }
        Value = ConvertFromJsonElement((JsonElement)value, this);
    }
}