using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbValueNode : DbTaskNode
{
    public DbValueNode(TaskNode node) : base(node){}
    public DbValueNode(){}

    public async override Task Process(ContextService context)
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
            Value = new NodeValue.InvalidValue($"In Node: invalid value type: {type}");
        }
    }
}
