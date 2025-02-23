using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbSendMessageNode : DbTaskNode
{
    public DbSendMessageNode(TaskNode node) : base(node)
    {
    }

    public DbSendMessageNode()
    {
    }

    public override NodeValue GetValue(DbTaskLink dbLink, IServerContext context)
    {
        if (Value.Type == NodeValueType.Waiting)
        {
            Process(context);
        }
        return Value;
    }

    public override void Process(IServerContext context)
    {
        Value = TargetLinks.FirstOrDefault()?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order},had no input");
        if(Value.Type != NodeValueType.Invalid)
        {
            if (StringVal2 == "String")
            {
                Value = new NodeValue.StringValue(StringVal3);
                context.SendToModule(LongVal1, StringVal1, StringVal3);
            }
            else if (StringVal2 == "Number")
            {
                Value = new NodeValue.NumberValue(DoubleVal1);
                context.SendToModule(LongVal1, StringVal1, DoubleVal1);
            }
            else if (StringVal2 == "Boolean")
            {
                Value = new NodeValue.BooleanValue(BoolVal1);
                context.SendToModule(LongVal1, StringVal1, BoolVal1);
            }
            else
            {
                Value = new NodeValue.InvalidValue($"Invalid type: {StringVal2}, in node: {Order}");
            }
        }

        //Debug.WriteLine($"GetValue SendMessage: {Value.Type}, value: {Value}, key: {StringVal1}");
    }
}