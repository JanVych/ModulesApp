using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModulesApp.Interfaces;
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

    public int SourceOrder { get; set; }
    public bool SourceInput { get; set; } = false;
    public bool SourceData { get; set; } = false;

    public int TargetOrder { get; set; }
    public bool TargetInput { get; set; } = false;
    public bool TargetData { get; set; } = false;


    public DbTaskLink(TaskPort sourcePort, DbTaskNode sourceNode, TaskPort targetPort, DbTaskNode targetNode)
    {
        Source = sourceNode;
        Target = targetNode;

        SourceOrder = sourcePort.Order;
        SourceInput = sourcePort.Input;
        SourceData = sourcePort.Data;

        TargetOrder = targetPort.Order;
        TargetInput = targetPort.Input;
        TargetData = targetPort.Data;
    }

    public DbTaskLink()
    {
    }

    public NodeValue GetValue(ContextService context)
    {
        return Source.GetValue(this, context);
    }
}