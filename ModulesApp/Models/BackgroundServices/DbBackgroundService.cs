using ModulesApp.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
            await Task.Delay(1000, _cancellationToken.Token);
            Console.WriteLine("The Service has started");

            try
            {
                while (!_cancellationToken.Token.IsCancellationRequested)
                {
                    await ExecuteAsync(serverContext);
                    LastRun = DateTime.Now;
                    foreach (var j in JsonData)
                    {
                        Console.WriteLine($"{j.Key}: {j.Value}");
                    }
                    Console.WriteLine();
                    JsonData.Clear();

                    await Task.Delay(Interval, _cancellationToken.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("The Service was canceled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
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

    protected void AddToMessage(string key, object? value)
    {
        if (value is not null)
        {
            JsonData.Add(key, value);
        }
    }

    public DbBackgroundService(){}
}
