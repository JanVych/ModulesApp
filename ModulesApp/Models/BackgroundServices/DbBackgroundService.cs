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
    protected CancellationTokenSource CancellationToken { get; private set; } = new();
    public DbBackgroundService() { }
    public override string ToString() => Name;

    public abstract Task ExecuteAsync();

    public async Task StartAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContextService>();

        Status = BackgroundServiceStatus.Running;
        LastRun = DateTime.Now;
        CancellationToken = new CancellationTokenSource();
        await context.UpdateFromBackgroundService(this);
        Console.WriteLine("The Service has started");
        try
        {
            while (!CancellationToken.Token.IsCancellationRequested)
            {
                Actions = await context.GetActionsAsync(this);
                LastRun = DateTime.Now;

                await ExecuteAsync();
                Actions.Clear();

                await context.UpdateFromBackgroundService(this);
                await context.ExecuteServerTasksAsync(this);

                await Task.Delay(Interval, CancellationToken.Token);
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
            CancellationToken.Dispose();
            Status = BackgroundServiceStatus.Stopped;
            await context.UpdateFromBackgroundService(this);
        }
    }

    public async Task StopAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var serverContextService = scope.ServiceProvider.GetRequiredService<ContextService>();
        Status = BackgroundServiceStatus.Cancelling;
        await serverContextService.UpdateFromBackgroundService(this);
        await CancellationToken.CancelAsync();
    }

    protected void AddMessage(string key, object? value)
    {
        if (value is not null)
        {
            Data.TryAdd(key, value);
        }
    }

    public Color GetStatusColor()
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
