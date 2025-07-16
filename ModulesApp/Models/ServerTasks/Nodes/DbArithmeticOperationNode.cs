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

        Value = OperationType switch
        {
            NodeArithmeticOperationType.Add => new NodeValue.NumberValue(nLeft.Value + nRight.Value),
            NodeArithmeticOperationType.Subtract => new NodeValue.NumberValue(nLeft.Value - nRight.Value),
            NodeArithmeticOperationType.Multiply => new NodeValue.NumberValue(nLeft.Value * nRight.Value),
            NodeArithmeticOperationType.Divide => nRight.Value == 0 ? 
                new NodeValue.InvalidValue($"In node: {Order}, division by zero!") :  
                new NodeValue.NumberValue(nLeft.Value / nRight.Value),
            _ => new NodeValue.InvalidValue($"In node: {Order}, invalid operation type!")
        };
    }
}
