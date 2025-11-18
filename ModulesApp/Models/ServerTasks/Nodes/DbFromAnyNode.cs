using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbFromAnyNode : DbTaskNode
{
    public DbFromAnyNode(TaskNode node) : base(node){}
    public DbFromAnyNode(){}

    public async override Task Process(ContextService context)
    {
        TargetType targetType = (TargetType)LongVal2;
        JsonElement? value;
        if (targetType == TargetType.Module)
        {
            value = await context.ModuleRepository.GetMessageDataAsync(LongVal1, StringVal1);
        }
        else if (targetType == TargetType.Service)
        {
            value = await context.BackgroundServiceRepository.GetMessageDataAsync(LongVal1, StringVal1);
        }
        else if (targetType == TargetType.Dashboard)
        {
            value = await context.DashboardRepository.GetMessageDataAsync(LongVal1, StringVal1);
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