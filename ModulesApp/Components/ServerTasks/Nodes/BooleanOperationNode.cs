using Blazor.Diagrams.Core.Geometry;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;

namespace ModulesApp.Components.ServerTasks.Nodes;

public class BooleanOperationNode : TaskNode
{
    public NodeBooleanOperationType OperationType => (NodeBooleanOperationType)SubType;

    public BooleanOperationNode(ContextService context, NodeBooleanOperationType operationType, Point? position = null)
        : base(context, position)
    {
        Type = NodeType.BooleanOperation;
        SubType = (int)operationType;
        AddPorts();
    }

    public BooleanOperationNode(ContextService context, DbTaskNode dbNode) : base(context, dbNode)
    {
        AddPorts();
    }

    private void AddPorts()
    {
        if(OperationType == NodeBooleanOperationType.Not)
        {
            InputType = NodeInputType.Single;
            //Input operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Center, dataType: NodeValueType.Boolean));
        }
        else
        {
            InputType = NodeInputType.Double;
            //Input right operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Top, dataType: NodeValueType.Any));
            //Input left operation port
            AddPort(new TaskPort(this, true, PortPositionAlignment.Bottom, dataType: NodeValueType.Any));
        }
        //Output port
        AddPort(new TaskPort(this, false, PortPositionAlignment.Center, dataType: NodeValueType.Boolean));
    }
}
