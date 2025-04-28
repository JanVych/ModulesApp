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
        Value = TargetLinks.FirstOrDefault(l => l.TargetData)?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no input");

        if (Value.Type != NodeValueType.Invalid)
        {
            Dictionary<string, object> data = [];
            if (Value is NodeValue.ArrayValue array)
            {
                data["Column2"] = array.ToStringList();
            }
            else
            {
                data["Title"] = StringVal1;
                data["Value"] = Value.ToString() ?? string.Empty;
            }
            
            context.DisplayValue(LongVal1, data);
        }
        else
        {
            Console.WriteLine(Value.ToString());
        }
    }
}
