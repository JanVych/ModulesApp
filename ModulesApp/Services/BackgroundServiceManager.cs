using ModulesApp.Models.BackgroundServices;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class BackgroundServiceManager
{
    private readonly IServiceProvider _serviceProvider;

    private List<DbBackgroundService> _backgroundServices = default!;
    private readonly object _lock = new();

    public BackgroundServiceManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Launch();
    }

    public void Launch()
    {
        using var scoop = _serviceProvider.CreateScope();
        var backgroundServiceService = scoop.ServiceProvider.GetRequiredService<BackgroundServiceService>();
        _backgroundServices = backgroundServiceService.GetAll();

        lock (_lock)
        {
            foreach (var backgroundService in _backgroundServices)
            {
                if (backgroundService.Status == BackgroundServiceStatus.Running)
                {
                    Task.Run(() => backgroundService.StartAsync(_serviceProvider));
                }
                else
                {
                    backgroundService.Status = BackgroundServiceStatus.Stopped;
                    backgroundServiceService.UpdateAsync(backgroundService).Wait();
                }
            }
        }
    }

    public void StartAsync(long serviceId)
    {
        DbBackgroundService? service;
        lock (_lock)
        {
            service = _backgroundServices.FirstOrDefault(x => x.Id == serviceId);
        }
        if (service is not null && service.Status == BackgroundServiceStatus.Stopped)
        {
            Task.Run(() => service.StartAsync(_serviceProvider));
        }
    }

    public async Task StopAsync(long serviceId)
    {
        DbBackgroundService? service;
        lock (_lock)
        {
            service = _backgroundServices.FirstOrDefault(x => x.Id == serviceId);
        }
        if (service is not null && service.Status == BackgroundServiceStatus.Running)
        {
            await service.StopAsync(_serviceProvider);
        }
    }

    public async Task RegisterServiceAsync(DbBackgroundService service, BackgroundServiceService backgroundServiceService)
    {
        await backgroundServiceService.AddAsync(service);
        lock (_lock)
        {
            _backgroundServices.Add(service);
        }
    }

    public async Task UnregisterServiceAync(long taskId, BackgroundServiceService backgroundServiceService)
    {
        var service = _backgroundServices.FirstOrDefault(x => x.Id == taskId);
        if (service is not null && service.Status == BackgroundServiceStatus.Stopped)
        {
            //await service.StopAsync();

            //while(service.IsRunning)
            //{
            //    await Task.Delay(100);
            //}
            lock (_lock)
            {
                _backgroundServices.Remove(service);
            }
            await backgroundServiceService.DeleteAsync(service);
        }
    }

    public async Task UpdateServiceAsync(DbBackgroundService service, BackgroundServiceService backgroundServiceService)
    {
        var index = -1;
        if (service.Status == BackgroundServiceStatus.Stopped)
        {
            lock (_lock)
            {
                index = _backgroundServices.FindIndex(x => x.Id == service.Id);
                if (index != -1)
                {
                    _backgroundServices[index] = service;
                }
            }
            if (index != -1)
            {
                await backgroundServiceService.UpdateAsync(service);
            }
        }
    }
}
