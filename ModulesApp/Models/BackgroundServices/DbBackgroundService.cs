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

    public static string GetDescription(BackgroundServiceType type)
    {
        return type switch
        {
            BackgroundServiceType.Goodwe => "For communication with the GoodWe inverter (tested on GW10KN-ET)\nConfiguration:\nIPv4 address of device\nPort of the device (typically 8899)\nAccepted actions:\nSetBatteryCharge - Watts\nSetBatteryDischarge - Watts",
            BackgroundServiceType.Cron => "Does not perform any job, used only to trigger tasks",
            BackgroundServiceType.Http => "Will try to query a JSON object from the provided URL",
            BackgroundServiceType.OteElectricityDam => "Get today's electricity data from the page https://www.ote-cr.cz/cs/kratkodobe-trhy/elektrina/denni-trh\nReturn:\nCurrentPrice - number\nTodayPrices - array\nTodayAmmounts - array\nDate - string",
            _ => "Unknown background service type."
        };
    }
}
