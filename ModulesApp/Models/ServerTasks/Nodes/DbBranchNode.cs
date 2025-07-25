using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks.Nodes;

public class DbBranchNode : DbTaskNode
{
    public DbBranchNode(TaskNode node) : base(node){}
    public DbBranchNode(){}

    public override void Process(ContextService context)
    {
        var triggerInputValue = GetInputValue(context, PortPositionAlignment.Top, "trigger");
        if (triggerInputValue.Type == NodeValueType.Invalid)
        {
            Value = triggerInputValue;
            return;
        }

        if (DataConvertor.ToBool(triggerInputValue.GetValue()))
        {
            Value = GetInputValue(context, PortPositionAlignment.Center, "data");
        }
        else
        {
            Value = GetInputValue(context, PortPositionAlignment.Bottom, "data");
        }
    }
}
