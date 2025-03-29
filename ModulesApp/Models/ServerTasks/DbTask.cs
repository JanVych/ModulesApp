using ModulesApp.Models.BackgroundServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ServerTasks;

public enum TaskType
{
    OnMessageRecived,
}

public enum TaskTargetType
{
    Module,
    BackgroundService,
}

[Table("Task")]
public class DbTask
{
    [Key]
    public long Id { get; set; }

    public TaskType Type { get; set; }

    public TaskTargetType TargetType { get; set; }

    public string Name { get; set; } = string.Empty;

    public int IntervalSeconds { get; set; }
    public DateTime LastRun { get; set; }

    public long? ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public DbModule? Module { get; set; }

    public long? BackgroundServiceId { get; set; }
    [ForeignKey("BackgroundServiceId")]
    public DbBackgroundService? BackgroundService { get; set; }

    public ICollection<DbTaskNode> Nodes { get; set; } = [];
}
