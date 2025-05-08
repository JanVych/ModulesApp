using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbValueNode : DbTaskNode
{
    public DbValueNode(TaskNode node) : base(node)
    {
    }

    public DbValueNode()
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
        NodeValueType type = (NodeValueType)LongVal1;
        if (type == NodeValueType.Number)
        {
            Value = new NodeValue.NumberValue(DoubleVal1);
        }
        else if (type == NodeValueType.String)
        {
            Value = new NodeValue.StringValue(StringVal1);
        }
        else if (type == NodeValueType.Boolean)
        {
            Value = new NodeValue.BooleanValue(BoolVal1);
        }
        else
        {
            Value = new NodeValue.InvalidValue($"Invalid value type: {type}, node: {Order}");
        }
    }
}
