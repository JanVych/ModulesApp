using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ConvertToNode : TaskNode
{
    public NodeConvertToType ConvertToType => (NodeConvertToType)SubType;
    public ConvertToNode(IServerContext context, NodeConvertToType type, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ConvertTo;
        SubType = (int)type;
        InputType = NodeInputType.Single;
        AddPorts();

    }

    public ConvertToNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        //Input port
        AddPort(new TaskPort(this, true, 0, data: true));
        //Output port
        AddPort(new TaskPort(this, false, 0, data: true));
    }
}
