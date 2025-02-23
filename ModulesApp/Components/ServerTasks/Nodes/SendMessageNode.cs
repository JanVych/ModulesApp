using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class SendMessageNode : TaskNode
{
    public List<Module> Modules { get; set; }
    public SendMessageNode(IServerContext context, Point? position = null) : base(context, position)
    {
        StringVal2 = "Number";
        Modules = context.GetAllModules();
        LongVal1 = Modules.FirstOrDefault()?.Id ?? 0;
        Type = NodeType.SendMessage;

        AddPort(new TaskPort(this, true, 0, data: false));
    }
    public SendMessageNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        Modules = context.GetAllModules();
        AddPort(new TaskPort(this, true, 0, data: false));
    }
}
