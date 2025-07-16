using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbConvertToNode : DbTaskNode
{
    public NodeValueType ConvertToType => (NodeValueType)SubType;

    public DbConvertToNode(TaskNode node) : base(node){}
    public DbConvertToNode(){}

    public override void Process(ContextService context)
    {
        Value = GetInputValue(context, PortPositionAlignment.Center);

        if (Value.Type != NodeValueType.Invalid)
        {
            if (ConvertToType == NodeValueType.String)
            {
                Value = new NodeValue.StringValue(DataConvertor.ToString(Value.GetValue()));
            }
            else if (ConvertToType == NodeValueType.Number)
            {
                Value = new NodeValue.NumberValue(DataConvertor.ToDouble(Value.GetValue()));
            }
            else if (ConvertToType == NodeValueType.Boolean)
            {
                Value = new NodeValue.BooleanValue(DataConvertor.ToBool(Value.GetValue()));
            }
            else if (ConvertToType == NodeValueType.Array)
            {
                if(Value is not NodeValue.ArrayValue)
                {
                    Value = new NodeValue.ArrayValue([Value]);
                }
            }
            else
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, type error, cannot convert to {ConvertToType}!");
            }
        }
    }

}
