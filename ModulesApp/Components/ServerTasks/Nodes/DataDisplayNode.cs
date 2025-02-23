using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class DataDisplayNode : TaskNode
{
    public List<DashBoardCard> Cards { get; set; } = [];
    public DataDisplayNode(IServerContext context, Point? position = null) : base(context, position)
    {
        Type = NodeType.DataDisplay;
        Cards = context.GetAllBoardCards();
        LongVal1 = Cards.FirstOrDefault()?.Id ?? 0;
        AddPort(new TaskPort(this, true, 0, data: true));
    }
    public DataDisplayNode(IServerContext context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPort(new TaskPort(this, true, 0, data: true));
        Cards = context.GetAllBoardCards();
    }
}
