using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbArithmeticSaturationNode : DbTaskNode
{
    public NodeArithmeticOperationType OperationType => (NodeArithmeticOperationType)SubType;

    public DbArithmeticSaturationNode(TaskNode node) : base(node){}
    public DbArithmeticSaturationNode(){}

    private NodeValue GetInputValue(ContextService context)
    {
        DbTaskLink? link = TargetLinks.FirstOrDefault();
        if (link == null)
        {
            return new NodeValue.InvalidValue($"node: {Order}, no input");
        }
        return link.GetValue(context);
    }

    public override void Process(ContextService context)
    {
        NodeValue value = GetInputValue(context);
        double? number = null;

        if (value.Type == NodeValueType.Invalid)
        {
            Value = value;
            return;
        }

        if(value is not NodeValue.NumberValue nValue)
        {
            Value = new NodeValue.InvalidValue($"node: {Order}, type error, input is not a number");
            return;
        }

        // ceiling
        if (BoolVal1 && DoubleVal1 < nValue.Value)
        {
            number = DoubleVal1;
        }

        // floor
        if(BoolVal2 && DoubleVal2 > nValue.Value)
        {
            number = DoubleVal2;
        }

        if (number == null)
        {
            Value = nValue;
        }
        else
        {
            Value = new NodeValue.NumberValue((double)number);
        }
    }
}
