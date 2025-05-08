using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class FromAnyNode : TaskNode
{
    public List<DbModule>? Modules { get; set; }
    public List<DbDashboardEntity>? Entities { get; set; }
    public List<DbBackgroundService>? Services { get; set; }

    public FromAnyNode(ContextService context, Point? position = null) : base(context, position) 
    {
        Type = NodeType.FromAny;

        Modules = context.GetAllModules();
        LongVal1 = Modules.FirstOrDefault()?.Id ?? 0;
        LongVal2 = (long)TargetType.Module;
        LongVal3 = (long)NodeValueType.Any;

        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }

    public FromAnyNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        TargetType type = (TargetType)LongVal2;
        if (type == TargetType.Module)
        {
            Modules = context.GetAllModules();
        }
        else if (type == TargetType.Service)
        {
            Services = context.GetAllBackgroundServices();
        }
        else if (type == TargetType.Dashboard)
        {
            Entities = context.GetAllDashBoardEntities();
        }

        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, data: true));
    }
}
