using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Services;

namespace ModulesApp.Models.ServerTasks;

[Table("TaskLink")]
public class DbTaskLink
{
    [Key]
    public long Id { get; set; }

    public long SourceNodeId { get; set; }
    public DbTaskNode Source { get; set; }

    public long TargetNodeId { get; set; }
    public DbTaskNode Target { get; set; }

    public PortPositionAlignment SourcePositionAlignment { get; set; }
    public bool SourceInput { get; set; } = false;
    public NodeValueType SourceDataType { get; set; } = NodeValueType.Any;

    public PortPositionAlignment TargetPositionAlignment { get; set; }
    public bool TargetInput { get; set; } = false;
    public NodeValueType TargetDataType { get; set; } = NodeValueType.Any;


    public DbTaskLink(TaskPort sourcePort, DbTaskNode sourceNode, TaskPort targetPort, DbTaskNode targetNode)
    {
        Source = sourceNode;
        Target = targetNode;

        SourcePositionAlignment = sourcePort.PositionAlignment;
        SourceInput = sourcePort.Input;
        SourceDataType = sourcePort.DataType;

        TargetPositionAlignment = targetPort.PositionAlignment;
        TargetInput = targetPort.Input;
        TargetDataType = targetPort.DataType;
    }

    public DbTaskLink()
    {
    }

    public NodeValue GetValue(ContextService context)
    {
        return Source.GetValue(this, context);
    }
}