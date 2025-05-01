using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
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
    public SendMessageNode(ContextService context, Point? position = null) : base(context, position)
    {
        Type = NodeType.SendMessage;

        Modules = context.GetAllModules();
        LongVal1 = Modules.FirstOrDefault()?.Id ?? 0;
        LongVal2 = (long)TargetType.Module;

        AddPorts(NodeInputType.Single);
    }
    public SendMessageNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        if (LongVal2 == (long)TargetType.Module)
        {
            Modules = context.GetAllModules();
        }
        else if (LongVal2 == (long)TargetType.Service)
        {
            Services = context.GetAllBackgroundServices();
        }
        else if (LongVal2 == (long)TargetType.DashboardEntity)
        {
            Entities = context.GetAllDashBoardEntities();
        }
        AddPorts(InputType);
    }

    public void AddPorts(NodeInputType type)
    {
        InputType = type;
        RemoveAllInputPorts();
        if (type == NodeInputType.Single)
        {
            //Trigger and data port
            AddPort(new TaskPort(this, true, 0, data: true));
        }
        else
        {
            //Trigger port
            AddPort(new TaskPort(this, true, 1, data: false));
            //Data port
            AddPort(new TaskPort(this, true, 2, data: true));
        }
    }
}
