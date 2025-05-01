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
        if (StringVal2 == "Number")
        {
            Value = new NodeValue.NumberValue(DoubleVal1);
        }
        else if (StringVal2 == "String")
        {
            Value = new NodeValue.StringValue(StringVal1);
        }
        else if (StringVal2 == "Boolean")
        {
            Value = new NodeValue.BooleanValue(BoolVal1);
        }
        else
        {
            Value = new NodeValue.InvalidValue($"Invalid Value Type, node: {Order}");
        }

    }
}
