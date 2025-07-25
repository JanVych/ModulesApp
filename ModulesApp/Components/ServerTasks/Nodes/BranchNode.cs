using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class BranchNode : TaskNode
{
    public List<DbDashboardEntity> Entities { get; set; } = [];
    public BranchNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.Branch;
        AddPorts();
    }

    public BranchNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    public void AddPorts()
    {
        //Trigger port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.NoData));
        //First data port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Any));
        //Secnd data port
        AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: NodeValueType.Any));

        //Output port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.Any));
    }
}
