using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbConvertToNode : DbTaskNode
{
    public NodeConvertToType ConvertToType => (NodeConvertToType)SubType;

    private DbTaskLink? Input => TargetLinks.FirstOrDefault();

    public DbConvertToNode(TaskNode node) : base(node)
    {
    }

    public DbConvertToNode()
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
        Value = Input?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no input");
        if (Value.Type != NodeValueType.Invalid)
        {
            if (ConvertToType == NodeConvertToType.String)
            {
                Value = new NodeValue.StringValue(DataConvertor.ToString(Value.GetValue()));
            }
            else if (ConvertToType == NodeConvertToType.Number)
            {
                Value = new NodeValue.NumberValue(DataConvertor.ToDouble(Value.GetValue()));
            }
            else if (ConvertToType == NodeConvertToType.Boolean)
            {
                Value = new NodeValue.BooleanValue(DataConvertor.ToBool(Value.GetValue()));
            }
            else if (ConvertToType == NodeConvertToType.Array)
            {
                if(Value is not NodeValue.ArrayValue)
                {
                    Value = new NodeValue.ArrayValue([Value]);
                }
            }
            else
            {
                Value = new NodeValue.InvalidValue($"node: {Order}, type error, cannot convert to {ConvertToType}");
            }
        }
    }

}
