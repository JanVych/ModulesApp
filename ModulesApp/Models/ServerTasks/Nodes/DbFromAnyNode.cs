using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbFromAnyNode : DbTaskNode
{
    public DbFromAnyNode(TaskNode node) : base(node){}
    public DbFromAnyNode(){}

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
            Value = new NodeValue.InvalidValue($"In node: {Order}, invalid source type: {targetType}!");
            return;
        }

        if (value is not JsonElement jValue)
        {
            Value = new NodeValue.InvalidValue($"In node {Order}, no such key:{StringVal1} in {targetType}!");
            return;
        }

        if (!NodeValue.IsValidType(jValue, (NodeValueType)LongVal3))
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, value is not {(NodeValueType)LongVal3}, but {jValue.ValueKind}!");
            return;
        }
        Value = NodeValue.CreateFromJsonElement((JsonElement)value, this);
    }
}