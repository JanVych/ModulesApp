using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ServerTasks;

public enum TaskType
{
    OnMessageRecived,
}

[Table("Task")]
public class DbTask
{
    [Key]
    public long Id { get; set; }

    public TaskType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public int IntervalSeconds { get; set; }
    public DateTime LastRun { get; set; }

    public long? ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public Module? Module { get; set; }

    public ICollection<DbTaskNode> Nodes { get; set; } = [];
}
