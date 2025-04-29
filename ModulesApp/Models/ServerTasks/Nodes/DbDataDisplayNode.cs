using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbDataDisplayNode : DbTaskNode
{
    public DbDataDisplayNode(TaskNode node) : base(node)
    {
    }

    public DbDataDisplayNode()
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
            Value = TargetLinks.FirstOrDefault(l => l.TargetData)?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no input");
            if (Value.Type != NodeValueType.Invalid)
            {
                context.SendToDashboardEntity(LongVal1, "Value", Value.GetValue());
            }
        }
        else
        {
            Console.WriteLine(Value.ToString());
        }
    }
}
