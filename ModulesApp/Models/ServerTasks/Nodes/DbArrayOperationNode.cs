using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbArrayOperationNode : DbTaskNode
{
    public NodeArrayOperationType OperationType => (NodeArrayOperationType)SubType;

    public DbArrayOperationNode(TaskNode node) : base(node)
    {
    }

    public DbArrayOperationNode()
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
        if (Value.Type == NodeValueType.Invalid)
        {
            return;
        }
        if(Value is NodeValue.ArrayValue array)
        {
            List<NodeValue> arrayCLone = array.GetValueClone();

            if (OperationType == NodeArrayOperationType.ArrayRemoveAt)
            {
                if (LongVal1 < 0 && LongVal1 > array.Value.Count - 1)
                {
                    Value = new NodeValue.InvalidValue($"node: {Order}, index out of range, index:{LongVal1}");
                }
                else
                {
                    arrayCLone.RemoveAt((int)LongVal1);
                   Value = new NodeValue.ArrayValue(arrayCLone);
                }
            }
            else if (OperationType == NodeArrayOperationType.ArraySlice)
            {
                if (LongVal1 < 0 || LongVal2 < 0 || LongVal1 > array.Value.Count - 1 || LongVal2 > array.Value.Count - 1 || LongVal1 > LongVal2)
                {
                    Value = new NodeValue.InvalidValue($"node: {Order}, index out of range, index1:{LongVal1}, index2:{LongVal2}");
                }
                else
                {
                    Value = new NodeValue.ArrayValue(arrayCLone.GetRange((int)LongVal1, (int)(LongVal2 - LongVal1 + 1)));
                }
            }
        }
        else
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, value is not array, type: {Value.Type}");
        }
    }
}
