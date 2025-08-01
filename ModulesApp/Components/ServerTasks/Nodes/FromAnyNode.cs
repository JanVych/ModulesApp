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

    public List<string>? Keys { get; set; }

    public FromAnyNode(ContextService context, Point? position = null) : base(context, position) 
    {
        Type = NodeType.FromAny;

        Modules = context.GetAllModules();
        LongVal1 = Modules.FirstOrDefault()?.Id ?? 0;
        LongVal2 = (long)TargetType.Module;
        LongVal3 = (long)NodeValueType.Any;
        SetKeys();

        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.Any));
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
        SetKeys();

        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: (NodeValueType)LongVal3));
    }

    public void SetKeys()
    {
        Keys = null;


        if ((TargetType)LongVal2 == TargetType.Module)
        {
            Keys = Modules?.FirstOrDefault(x => x.Id == (int)LongVal1)?.Data.Select(v => v.Key).ToList();
        }
        else if ((TargetType)LongVal2 == TargetType.Dashboard)
        {
           Keys = Entities?.FirstOrDefault(x => x.Id == (int)LongVal1)?.Data.Select(v => v.Key).ToList();
        }
        else if ((TargetType)LongVal2 == TargetType.Service)
        {
           Keys = Services?.FirstOrDefault(x => x.Id == (int)LongVal1)?.MessageData.Select(v => v.Key).ToList();
        }
    }
}
