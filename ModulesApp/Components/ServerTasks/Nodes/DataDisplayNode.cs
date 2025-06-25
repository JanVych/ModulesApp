using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class DataDisplayNode : TaskNode
{
    public List<DbDashboardEntity> Entities { get; set; } = [];
    public DataDisplayNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.DataDisplay;
        Entities = context.GetAllDashBoardEntities();
        LongVal1 = Entities.FirstOrDefault()?.Id ?? 0;
        AddPorts(NodeInputType.Single);
    }

    public DataDisplayNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts(InputType);
        Entities = context.GetAllDashBoardEntities();
    }

    public void AddPorts(NodeInputType type)
    {
        InputType = type;
        RemoveAllInputPorts();
        if (type == NodeInputType.Single)
        {
            //Trigger and data port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Any));
        }
        else
        {
            //Trigger port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.NoData));
            //Data port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: NodeValueType.Any));
        }
    }
}
