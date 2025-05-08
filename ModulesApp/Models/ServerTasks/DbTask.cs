using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ServerTasks;

public enum TaskType
{
    OnMessageRecived,
}

public enum TargetType
{
    None,
    Module,
    Service,
    Dashboard,
}

[Table("Task")]
public class DbTask
{
    [Key]
    public long Id { get; set; }

    public TaskType Type { get; set; }

    public TargetType TriggerSourceType { get; set; } = TargetType.None;

    public string Name { get; set; } = string.Empty;

    public int IntervalSeconds { get; set; }
    public DateTime LastRun { get; set; }

    public long? ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public DbModule? Module { get; set; }

    public long? BackgroundServiceId { get; set; }
    [ForeignKey("BackgroundServiceId")]
    public DbBackgroundService? BackgroundService { get; set; }

    public long? DashboardEntityId { get; set; }
    [ForeignKey("DashboardEntityId")]
    public DbDashboardEntity? DashboardEntity { get; set; }

    public ICollection<DbTaskNode> Nodes { get; set; } = [];
}
