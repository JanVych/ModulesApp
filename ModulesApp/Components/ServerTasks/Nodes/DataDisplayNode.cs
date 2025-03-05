using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class DataDisplayNode : TaskNode
{
    public List<DbDashboardEntity> Entities { get; set; } = [];
    public DataDisplayNode(IServerContext context, Point? position = null) : base(context, position)
    {
        Type = NodeType.DataDisplay;
        Entities = context.GetAllDashBoardEntities();
        LongVal1 = Entities.FirstOrDefault()?.Id ?? 0;
        AddPort(new TaskPort(this, true, 0, data: true));
    }
    public DataDisplayNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, true, 0, data: true));
        Entities = context.GetAllDashBoardEntities();
    }
}
