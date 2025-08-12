using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbArrayOperationNode : DbTaskNode
{
    public NodeArrayOperationType OperationType => (NodeArrayOperationType)SubType;

    public DbArrayOperationNode(TaskNode node) : base(node){}
    public DbArrayOperationNode(){}

    public override void Process(ContextService context)
    {
        NodeValue? arrayValue = null;
        NodeValue? secondInput = null;

        if (SubType == (int)NodeArrayOperationType.ArrayCreate)
        {
            Value = new NodeValue.ArrayValue([]);
            return;
        }

        if (InputType == NodeInputType.Double)
        {
            arrayValue = GetInputValue(context, PortPositionAlignment.Top);
            secondInput = GetInputValue(context, PortPositionAlignment.Bottom);
        }
        else if(InputType == NodeInputType.Single)
        {
            arrayValue = GetInputValue(context, PortPositionAlignment.Center);
        }
        

        if ( arrayValue!= null && arrayValue.Type == NodeValueType.Invalid)
        {
            Value = arrayValue;
            return;
        }
        if (secondInput != null && secondInput.Type == NodeValueType.Invalid)
        {
            Value = secondInput;
            return;
        }

        if (arrayValue is not NodeValue.ArrayValue aValue)
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, type error, input is not an array!");
            return;
        }

        List<NodeValue> arrayCLone = aValue.GetValueClone();
        if (OperationType == NodeArrayOperationType.ArrayRemoveAt)
        {
            if(InputType == NodeInputType.Double)
            {
                if (secondInput is not NodeValue.NumberValue numberValue)
                {
                    Value = new NodeValue.InvalidValue($"In node: {Order}, type error, second input is not a number!");
                    return;
                }
                LongVal1 = (long)numberValue.Value;
            }
            if (LongVal1 < 0 || LongVal1 > aValue.Value.Count - 1)
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, index out of range, index:{LongVal1} !");
            }
            else
            {
                arrayCLone.RemoveAt((int)LongVal1);
                Value = new NodeValue.ArrayValue(arrayCLone);
            }
        }
        else if (OperationType == NodeArrayOperationType.ArraySlice)
        {
            if (LongVal1 < 0 || LongVal2 < 0 || LongVal1 > aValue.Value.Count - 1 || LongVal2 > aValue.Value.Count - 1 || LongVal1 > LongVal2)
            {
                Value = new NodeValue.InvalidValue($"In node: {Order}, index out of range, index1:{LongVal1}, index2:{LongVal2} !");
            }
            else
            {
                Value = new NodeValue.ArrayValue(arrayCLone.GetRange((int)LongVal1, (int)(LongVal2 - LongVal1 + 1)));
            }
        }
        else if (OperationType == NodeArrayOperationType.ArrayAppend && secondInput != null)
        {
            arrayCLone.Add(secondInput);
            Value = new NodeValue.ArrayValue(arrayCLone);
        }

        else
        {
            Value = new NodeValue.InvalidValue($"In node: {Order}, operation type error, type:{OperationType} !");
        }
    }
}
