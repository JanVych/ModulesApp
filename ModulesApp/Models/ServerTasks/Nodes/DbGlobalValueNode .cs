using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;
using System.Text.Json;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbGlobalValueNode : DbTaskNode
{
    public NodeModeType NodeModeType => (NodeModeType)SubType;
    public DbGlobalValueNode(TaskNode node) : base(node){}
    public DbGlobalValueNode(){}

    public async override Task Process(ContextService context)
    {
        if(NodeModeType == NodeModeType.Get)
        {
            var global = context.ServerTaskRepository.GetGlobalValue(StringVal1, StringVal2);
            if(global?.Value is not JsonElement jValue)
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, global group: {StringVal1}, key: {StringVal2} does not exist");
                return;
            }

            if (!NodeValue.IsValidType(jValue, (NodeValueType)LongVal1))
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, value is not {(NodeValueType)LongVal1}, but: {jValue.ValueKind}!");
                return;
            }

            Value = NodeValue.CreateFromJsonElement(jValue, this);
        }

        else if(NodeModeType == NodeModeType.Set)
        {
            if (InputType == NodeInputType.Double)
            {
                var triggerInputValue = await GetInputValue(context, PortPositionAlignment.Top, "trigger");
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
                Value = await GetInputValue(context, PortPositionAlignment.Bottom, "data");
            }
            else
            {
                Value = await GetInputValue(context, PortPositionAlignment.Center, "data");
            }
            if (Value.Type == NodeValueType.Invalid)
            {
                return;
            }
            if (string.IsNullOrEmpty(StringVal2))
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, key can not be empty!");
                return;
            }
            context.ServerTaskRepository.SetGlobalValue(StringVal1, StringVal2, Value.GetValue());
        }
    }
}
