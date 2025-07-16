using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbBooleanOperationNode : DbTaskNode
{
    public NodeBooleanOperationType OperationType => (NodeBooleanOperationType)SubType;

    public DbBooleanOperationNode(TaskNode node) : base(node){}
    public DbBooleanOperationNode(){}

    public override void Process(ContextService context)
    {
        NodeValue leftValue = GetInputLeftValue(context);
        var left = false;
        var right = false;

        if (leftValue.Type == NodeValueType.Invalid)
        {
            Value = leftValue;
            return;
        }
        left = DataConvertor.ToBool(leftValue.GetValue());

        if (OperationType == NodeBooleanOperationType.Not)
        {
            Value = new NodeValue.BooleanValue(!left);
        }

        // lazy evaluation
        else if ((left && OperationType == NodeBooleanOperationType.Or) || (!left && OperationType == NodeBooleanOperationType.Nand))
        {
            Value = new NodeValue.BooleanValue(true);
        }
        else if((!left && OperationType == NodeBooleanOperationType.And) || (left && OperationType == NodeBooleanOperationType.Nor))
        {
            Value = new NodeValue.BooleanValue(false);
        }

        else
        {
            NodeValue rightValue = GetInputValue(context, PortPositionAlignment.Bottom, "right");
            if (rightValue.Type == NodeValueType.Invalid)
            {
                Value = rightValue;
                return;
            }
            right = DataConvertor.ToBool(rightValue.GetValue());
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
