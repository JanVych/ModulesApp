using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbArithmeticOperationNode : DbTaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;
    private DbTaskLink? Left => TargetLinks.FirstOrDefault(l => l.TargetOrder == 1);
    private DbTaskLink? Right => TargetLinks.FirstOrDefault(l => l.TargetOrder == 2);

    public DbArithmeticOperationNode(TaskNode node) : base(node)
    {
    }
    public DbArithmeticOperationNode()
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
        NodeValue leftValue;
        if (InputType == NodeInputType.Single)
        {
            leftValue = TargetLinks.FirstOrDefault()?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no left input");
        }
        else
        {
            leftValue = Left?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no left input");
        }
        
        if(leftValue.Type == NodeValueType.Invalid)
        {
            Value = leftValue;
            return;
        }

        NodeValue? rightValue = InputType == NodeInputType.Double
            ? Right?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no right input")
            : new NodeValue.NumberValue(DoubleVal1);

        if (leftValue.Type == NodeValueType.Invalid && rightValue.Type == NodeValueType.Invalid)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, both left and right are not numbers");
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
