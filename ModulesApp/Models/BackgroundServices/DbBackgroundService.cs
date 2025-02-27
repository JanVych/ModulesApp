using ModulesApp.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ModulesApp.Models.BackgroundServices;

public enum BackgroundServiceType
{
    Goodwe,
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

    public Dictionary<string, object> JsonData { get; set; } = [];

    public bool IsRunning { get; private set; } = false;

    [NotMapped]
    protected CancellationTokenSource _cancellationToken = new();

    public abstract Task ExecuteAsync(IServerContext serverContext);

    public async Task StartAsync(IServerContext serverContext)
    {
        if (!IsRunning)
        {
            IsRunning = true;
            _cancellationToken = new CancellationTokenSource();

            try
            {
                while (!_cancellationToken.Token.IsCancellationRequested)
                {
                    await ExecuteAsync(serverContext);
                    LastRun = DateTime.Now;
                    await Task.Delay(Interval, _cancellationToken.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("The Service was canceled");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                IsRunning = false;
                _cancellationToken.Dispose();
            }
        }
    }

    public async Task StopAsync()
    {
        if(IsRunning)
        {
            await _cancellationToken.CancelAsync();
        }   
    }

    public DbBackgroundService(){}
}
