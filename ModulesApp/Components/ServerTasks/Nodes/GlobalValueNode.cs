using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class GlobalValueNode : TaskNode
{
    public NodeModeType NodeModeType => (NodeModeType)SubType;

    private List<DbGlobalValue> Globals { get; set;} = [];
    public List<string>? Groups { get; set; }
    public List<string>? Keys => Globals.Where(x => x.Group == StringVal1).Select(x => x.Key).ToList();

    public GlobalValueNode(ContextService context, NodeModeType subType,  Point? position = null) : base(context, position)
    {
        Type = NodeType.GlobalValue;
        SubType = (int)subType;
        InputType = NodeInputType.Single;
        LongVal1 = (long)NodeValueType.String;

        Initialize(context);
    }

    public GlobalValueNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        Initialize(context);
    }

    public void Initialize(ContextService context)
    {
        LoadStrings(context);
        if(NodeModeType == NodeModeType.Get)
        {
            AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: (NodeValueType)LongVal1));
        }
        else if(NodeModeType == NodeModeType.Set && InputType == NodeInputType.Single)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center));
        }
        else
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.Boolean));
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: NodeValueType.Any));
        }
    }

    public void AddPorts()
    {
        RemoveAllInputPorts();
        if (NodeModeType == NodeModeType.Get)
        {
            AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: (NodeValueType)LongVal1));
        }
        else if (NodeModeType == NodeModeType.Set && InputType == NodeInputType.Single)
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center));
        }
        else
        {
            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.Boolean));
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: NodeValueType.Any));
        }
    }

    public void LoadStrings(ContextService context)
    {
        Globals = context._serverTaskService.GetAllGlobalValues();
        Groups = Globals.Select(g => g.Group).Distinct().ToList();
    }
}