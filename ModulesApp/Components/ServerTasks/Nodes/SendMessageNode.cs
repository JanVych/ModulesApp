using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class SendMessageNode : TaskNode
{
    public List<DbModule>? Modules { get; set; }
    public List<DbDashboardEntity>? Entities { get; set; }
    public List<DbBackgroundService>? Services { get; set; }
    public List<string>? Keys { get; set; }

    public SendMessageNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.SendMessage;

        Modules = context.GetAllModules();
        LongVal1 = Modules.FirstOrDefault()?.Id ?? 0;
        LongVal2 = (long)TargetType.Module;
        SetKeys();
        AddPorts(NodeInputType.Single);
    }
    public SendMessageNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
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
        AddPorts(InputType);
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
