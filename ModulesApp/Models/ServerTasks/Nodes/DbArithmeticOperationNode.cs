using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbArithmeticOperationNode : DbTaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;

    public DbArithmeticOperationNode(TaskNode node) : base(node){}
    public DbArithmeticOperationNode(){}

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

        if (rightValue.Type == NodeValueType.Invalid)
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
        else if(rightValue.Type != NodeValueType.Number)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, right is not a number");
            return;
        }
        else if (leftValue.Type != NodeValueType.Number)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, left is not a number");
            return;
        }

        var nLeft = (NodeValue.NumberValue)leftValue;
        var nRight = (NodeValue.NumberValue)rightValue;

        Value = OperationType switch
        {
            NodeArithmeticOperationType.Add => new NodeValue.NumberValue(nLeft.Value + nRight.Value),
            NodeArithmeticOperationType.Subtract => new NodeValue.NumberValue(nLeft.Value - nRight.Value),
            NodeArithmeticOperationType.Multiply => new NodeValue.NumberValue(nLeft.Value * nRight.Value),
            NodeArithmeticOperationType.Divide => nRight.Value == 0 ? 
                new NodeValue.InvalidValue($"node: {Order}, division by zero") :  
                new NodeValue.NumberValue(nLeft.Value / nRight.Value),
            _ => new NodeValue.InvalidValue($"node: {Order}, invalid operation type")
        };
    }
}
