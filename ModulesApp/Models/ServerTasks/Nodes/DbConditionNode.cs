using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbConditionNode : DbTaskNode
{
    public NodeConditionType ConditionType => (NodeConditionType)SubType;

    public DbConditionNode(TaskNode node) : base(node){}

    public DbConditionNode(){}

    private NodeValue GetLeftValue(ContextService context)
    {
        DbTaskLink? link;
        if (InputType == NodeInputType.Single)
        {
            link = TargetLinks.FirstOrDefault(l => l.TargetInput);
        }
        else
        {
            link = TargetLinks.FirstOrDefault(l => l.TargetInput && l.TargetPositionAlignment == PortPositionAlignment.Top);
        }
        if (link == null)
        {
            return new NodeValue.InvalidValue($"node: {Order}, no left input");
        }
        return link.GetValue(context);
    }

    private NodeValue GetRightValue(ContextService context)
    {
        if (InputType == NodeInputType.Double)
        {
            DbTaskLink? link = TargetLinks.FirstOrDefault(l => l.TargetInput && l.TargetPositionAlignment == PortPositionAlignment.Bottom);
            if (link == null)
            {
                return new NodeValue.InvalidValue($"node: {Order}, no right input");
            }
            return link.GetValue(context);
        }
        return new NodeValue.NumberValue(DoubleVal1);
    }

    public override void Process(ContextService context)
    {
        NodeValue leftValue = GetLeftValue(context);
        NodeValue rightValue = GetRightValue(context);

        if(rightValue.Type == NodeValueType.Invalid)
        {
            Value = rightValue;
            return;
        }

        if (leftValue.Type == NodeValueType.Invalid)
        {
            Value = leftValue;
            return;
        }

        if (leftValue.Type != NodeValueType.Number && rightValue.Type != NodeValueType.Number)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, left and right are not numbers");
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
}
