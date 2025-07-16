using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbConditionNode : DbTaskNode
{
    public NodeConditionType ConditionType => (NodeConditionType)SubType;

    public DbConditionNode(TaskNode node) : base(node){}
    public DbConditionNode(){}

    public override void Process(ContextService context)
    {
        NodeValue leftValue = GetInputLeftValue(context);
        if (leftValue.Type == NodeValueType.Invalid)
        {
            Value = leftValue;
            return;
        }
        if (leftValue is not NodeValue.NumberValue nLeft)
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, type error, left input is not a number!");
            return;
        }

        NodeValue rightValue = InputType == NodeInputType.Double
            ? GetInputValue(context, PortPositionAlignment.Bottom, "right")
            : new NodeValue.NumberValue(DoubleVal1);

        if (rightValue.Type == NodeValueType.Invalid)
        {
            Value = rightValue;
            return;
        }
        if (rightValue is not NodeValue.NumberValue nRight)
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, type error, right input is not a number!");
            return;
        }

        var result = ConditionType switch
        {
            NodeConditionType.Equal => nLeft.Value == nRight.Value,
            NodeConditionType.NotEqual => nLeft.Value != nRight.Value,
            NodeConditionType.Less => nLeft.Value < nRight.Value,
            NodeConditionType.LessOrEqual => nLeft.Value <= nRight.Value,
            NodeConditionType.Greater => nLeft.Value > nRight.Value,
            NodeConditionType.GreaterOrEqual => nLeft.Value >= nRight.Value,
            _ => false
        };
        Value = new NodeValue.BooleanValue(result);
    }
}
