using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class ConvertToNode : TaskNode
{
    public NodeValueType ConvertToType => (NodeValueType)SubType;
    public ConvertToNode(ContextService context, NodeValueType convertToType, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.ConvertTo;
        SubType = (int)convertToType;
        InputType = NodeInputType.Single;
        AddPorts();

    }

    public ConvertToNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        //Input port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Center, data: true));
        //Output port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }
}
