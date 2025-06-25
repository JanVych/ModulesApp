using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbBooleanOperationNode : DbTaskNode
{
    public NodeBooleanOperationType OperationType => (NodeBooleanOperationType)SubType;

    public DbBooleanOperationNode(TaskNode node) : base(node){}
    public DbBooleanOperationNode(){}

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

        if (leftValue.Type == NodeValueType.Invalid)
        {
            Value = leftValue;
            return;
        }
        var left = NodeValue.GetBooleanValue(leftValue);

        if (OperationType == NodeBooleanOperationType.Not)
        {
            Value = new NodeValue.BooleanValue(!left);
        }

        else
        {
            NodeValue rightValue = GetRightValue(context);
            if (leftValue.Type == NodeValueType.Invalid)
            {
                Value = leftValue;
                return;
            }
            var right = NodeValue.GetBooleanValue(rightValue);
            Value = new NodeValue.BooleanValue(ResolveOperation(left, right));
        }
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
