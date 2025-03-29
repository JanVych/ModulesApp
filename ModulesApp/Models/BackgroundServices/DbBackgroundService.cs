using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.BackgroundServices;

public enum BackgroundServiceType
{
    Goodwe,
    Test
}

public enum BackgroundServiceStatus
{
    Running,
    Stopped,
    Error,
    Cancelling,
}

[Table("BackgroundService")]
public abstract class DbBackgroundService
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public BackgroundServiceType Type { get; set; }
    public TimeSpan Interval { get; set; }
    public DateTime LastRun { get; set; }
    public Dictionary<string, object> Data { get; set; } = [];
    public BackgroundServiceStatus Status { get; set; } = BackgroundServiceStatus.Stopped;

    public ICollection<DbAction> Actions { get; set; } = [];
    public ICollection<DbTask> ServerTasks { get; set; } = [];
    public string IntervalString => Interval.TotalSeconds.ToString() + " s";

    [NotMapped]
    protected CancellationTokenSource _cancellationToken = new();
    public DbBackgroundService() { }
    public override string ToString() => Name;

    public abstract Task ExecuteAsync();

    public async Task StartAsync(BackgroundServiceManager backgroundServiceManager)
    {
        Status = BackgroundServiceStatus.Running;
        _cancellationToken = new CancellationTokenSource();
        await backgroundServiceManager.UpdateFromService(this);
        Console.WriteLine("The Service has started");
        try
        {
            while (!_cancellationToken.Token.IsCancellationRequested)
            {
                Actions = await backgroundServiceManager.GetActions(this);
                LastRun = DateTime.Now;

                await ExecuteAsync();
                Actions.Clear();

                await backgroundServiceManager.UpdateFromService(this);
                await backgroundServiceManager.ExecuteServerTasks(this);

                await Task.Delay(Interval, _cancellationToken.Token);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The Service was stopped");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            _cancellationToken.Dispose();
            Status = BackgroundServiceStatus.Stopped;
            await backgroundServiceManager.UpdateFromService(this);
        }
    }

    public async Task StopAsync(BackgroundServiceManager backgroundServiceManager)
    {
        Status = BackgroundServiceStatus.Cancelling;
        await backgroundServiceManager.UpdateFromService(this);
        await _cancellationToken.CancelAsync();
    }

    protected void AddMessage(string key, object? value)
    {
        if (value is not null)
        {
            Data.TryAdd(key, value);
        }
    }

    public Color StatusColor()
    {
        return Status switch
        {
            BackgroundServiceStatus.Running => Color.Success,
            BackgroundServiceStatus.Stopped => Color.Primary,
            BackgroundServiceStatus.Error => Color.Error,
            BackgroundServiceStatus.Cancelling => Color.Secondary,
            _ => Color.Error,
        };
    }
}
