using ModulesApp.Models.ServerTasks;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.BackgroundServices;

public enum BackgroundServiceType
{
    Goodwe,
    Cron,
    Http,
    OteElectricityDam
}

public enum BackgroundServiceStatus
{
    Active,
    Paused,
    Error,
    Cancelling,
}

[Table("BackgroundService")]
public class DbBackgroundService
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string CronExpression { get; set; } = "0/5 * * ? * * *";
    public string Description { get; set; } = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Quos blanditiis tenetur unde suscipit, quam beatae rerum inventore consectetur, neque doloribus, cupiditate numquam dignissimos laborum fugiat deleniti? Eum quasi quidem quibusdam.";
    public BackgroundServiceType Type { get; set; } = BackgroundServiceType.Cron;
    public BackgroundServiceStatus Status { get; set; } = BackgroundServiceStatus.Paused;

    public Dictionary<string, object?> ConfigurationData { get; set; } = [];
    public Dictionary<string, object?> MessageData { get; set; } = [];

    public ICollection<DbAction> Actions { get; set; } = [];
    public ICollection<DbTask> ServerTasks { get; set; } = [];

    public DbBackgroundService() { }

    public override string ToString() => Name;

    public Color GetStatusColor()
    {
        return Status switch
        {
            BackgroundServiceStatus.Active => Color.Success,
            BackgroundServiceStatus.Paused => Color.Primary,
            BackgroundServiceStatus.Error => Color.Error,
            BackgroundServiceStatus.Cancelling => Color.Secondary,
            _ => Color.Error,
        };
    }
}
