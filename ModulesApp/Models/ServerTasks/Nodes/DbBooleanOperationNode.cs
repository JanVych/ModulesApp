using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbBooleanOperationNode : DbTaskNode
{
    public NodeBooleanOperationType OperationType => (NodeBooleanOperationType)SubType;
    private DbTaskLink? Left => TargetLinks.FirstOrDefault(l => l.TargetPositionAlignment == PortPositionAlignment.Start);
    private DbTaskLink? Right => TargetLinks.FirstOrDefault(l => l.TargetPositionAlignment == PortPositionAlignment.End);

    public DbBooleanOperationNode(TaskNode node) : base(node)
    {
    }
    public DbBooleanOperationNode()
    {
    }

    public override NodeValue GetValue(DbTaskLink dbLink, ContextService context)
    {
        if (Value.Type == NodeValueType.Waiting)
        {
            Process(context);
        }
        return Value;
    }

    public override void Process(ContextService context)
    {
        NodeValue leftValue;
        if(OperationType == NodeBooleanOperationType.Not)
        {
            leftValue = TargetLinks.FirstOrDefault()?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no left input");
            if (leftValue.Type == NodeValueType.Invalid)
            {
                Value = leftValue;
                return;
            }
            var left = GetBooleanValue(leftValue);
            Value = new NodeValue.BooleanValue(!left);
        }
        else
        {
            leftValue = Left?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no left input");

            if (leftValue.Type == NodeValueType.Invalid)
            {
                Value = leftValue;
                return;
            }

            NodeValue? rightValue = Right?.GetValue(context) ?? new NodeValue.InvalidValue($"node: {Order}, no right input");
            if (rightValue.Type == NodeValueType.Invalid)
            {
                Value = rightValue;
                return;
            }
            var left = GetBooleanValue(leftValue);
            var right = GetBooleanValue(rightValue);
            Value = new NodeValue.BooleanValue(ResolveOperation(left, right));
        }
    }

    private bool GetBooleanValue(NodeValue value)
    {
        return value.Type switch
        {
            NodeValueType.Boolean => ((NodeValue.BooleanValue)value).Value,
            NodeValueType.String => bool.TryParse(((NodeValue.StringValue)value).Value, out var result) && result,
            NodeValueType.Number => ((NodeValue.NumberValue)value).Value != 0,
            NodeValueType.Array => ((NodeValue.ArrayValue)value).Value.Count > 0,
            _ => false
        };
    }

    private bool ResolveOperation(bool left, bool right)
    {
        return OperationType switch
        {
            NodeBooleanOperationType.And => left && right,
            NodeBooleanOperationType.Or => left || right,
            NodeBooleanOperationType.Xor => left ^ right,
            NodeBooleanOperationType.Nor => !(left || right),
            NodeBooleanOperationType.Nand => !(left && right),
            NodeBooleanOperationType.Xnor => !(left ^ right),
            _ => false
        };
    }
}
