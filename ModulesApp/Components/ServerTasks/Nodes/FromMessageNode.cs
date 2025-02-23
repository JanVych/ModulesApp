using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class FromMessageNode : TaskNode
{
    public FromMessageNode(IServerContext context, Point? position = null) : base(context, position) 
    {
        StringVal2 = "any";
        Type = NodeType.FromMessage;
        AddPort(new TaskPort(this, false, 0, data: true));
    }

    public FromMessageNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, false, 0, data: true));
    }
}
