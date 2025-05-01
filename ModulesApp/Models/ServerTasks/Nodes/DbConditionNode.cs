using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbConditionNode : DbTaskNode
{
    public NodeConditionType ConditionType => (NodeConditionType)SubType;

    private DbTaskLink? Left => TargetLinks.FirstOrDefault(l => l.TargetData);
    private DbTaskLink? Right => TargetLinks.FirstOrDefault(l => !l.TargetData);

    public DbConditionNode(TaskNode node) : base(node)
    {
    }

    public DbConditionNode()
    {
    }

    public override NodeValue GetValue(DbTaskLink dbLink, ContextService context)
    {
        if (Value.Type == NodeValueType.Waiting)
        {
            Process(context);
        }

        NodeValue value = (Value.Type == NodeValueType.Invalid ||
                          (dbLink.SourceOrder == 1 && Result) ||
                          (dbLink.SourceOrder == 2 && !Result))
            ? Value
            : new NodeValue.InvalidValue($"node: {Order}, condition invalid");

        //Debug.WriteLine($"GetValue Condition: {value.Type}, value: {value}");
        return value;
    }

    public override void Process(ContextService context)
    {
        NodeValue leftValue = Left?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no left input");
        Value = leftValue;
        if (leftValue.Type == NodeValueType.Invalid)
        {
            return;
        }

        NodeValue? rightValue = InputType == NodeInputType.Double
            ? Right?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no right input")
            : new NodeValue.NumberValue(DoubleVal1);

        if (rightValue.Type == NodeValueType.Invalid)
        {
            Value = rightValue;
            return;
        }

        if (leftValue.Type == NodeValueType.Invalid && rightValue.Type == NodeValueType.Invalid)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, both left and right are not numbers");
            return;
        }
        else if (rightValue.Type != NodeValueType.Number)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, right is not a number");
            return;
        }
        else if (leftValue.Type != NodeValueType.Number)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, left is not a number");
            return;
        }
        else
        {
            var nLeft = (NodeValue.NumberValue)leftValue;
            var nRight = (NodeValue.NumberValue)rightValue;

            Result = ConditionType switch
            {
                NodeConditionType.Equal => nLeft.Value == nRight.Value,
                NodeConditionType.NotEqual => nLeft.Value != nRight.Value,
                NodeConditionType.Less => nLeft.Value < nRight.Value,
                NodeConditionType.LessOrEqual => nLeft.Value <= nRight.Value,
                NodeConditionType.Greater => nLeft.Value > nRight.Value,
                NodeConditionType.GreaterOrEqual => nLeft.Value >= nRight.Value,
                _ => false
            };
        }
    }

}
