using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbSendMessageNode : DbTaskNode
{
    public DbSendMessageNode(TaskNode node) : base(node)
    {
    }

    public DbSendMessageNode()
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
        NodeValue conditional;
        if (InputType == NodeInputType.Double)
        {
            conditional = TargetLinks.FirstOrDefault(n => !n.TargetData)?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order},had no input");
            if (conditional.Type == NodeValueType.Invalid)
            {
                Value = conditional;
            }
        }
        if (Value.Type != NodeValueType.Invalid)
        {
            Value = TargetLinks.FirstOrDefault(n => n.TargetData)?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order},had no data input");
            if (Value.Type != NodeValueType.Invalid)
            {
                if (LongVal2 == (long)TargetType.Module)
                {
                    context.SendToModule(LongVal1, StringVal1, Value.GetValue());
                }
                else if (LongVal2 == (long)TargetType.Service)
                {
                    context.SendToBackgroundService(LongVal1, StringVal1, Value.GetValue());
                }
                else if (LongVal2 == (long)TargetType.Dashboard)
                {
                    context.SendToDashboardEntity(LongVal1, StringVal1, Value.GetValue());
                }
                else
                {
                    Value = new NodeValue.InvalidValue($"Invalid type: {(TargetType)LongVal2}, in node: {Order}");
                }
            }
        }
    }
}