using ModulesApp.Models.BackgroundServices;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class BackgroundServiceManager
{
    private readonly BackgroundServiceService _backgroundServiceService;
    private readonly ServerContextService _serverContextService;

    private readonly List<DbBackgroundService> _backgroundServices;
    private readonly object _lock = new();


    public BackgroundServiceManager(BackgroundServiceService backgroundServiceService , ServerContextService serverContextService)
    {
        _backgroundServiceService = backgroundServiceService;
        _serverContextService = serverContextService;

        _backgroundServices = _backgroundServiceService.GetAll();
        Launch();
    }

    private void Launch()
    {
        lock (_lock)
        {
            foreach (var backgroundService in _backgroundServices)
            {
                if (backgroundService.Status == BackgroundServiceStatus.Running)
                {
                    Task.Run(() => backgroundService.StartAsync(_serverContextService));
                }
                else
                {
                    backgroundService.Status = BackgroundServiceStatus.Stopped;
                    _backgroundServiceService.UpdateAsync(backgroundService).Wait();
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
            Task.Run(() => service.StartAsync(_serverContextService));
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
            await service.StopAsync(_serverContextService);
        }
    }

    public async Task RegisterServiceAsync(DbBackgroundService service)
    {
        await _backgroundServiceService.AddAsync(service);
        lock (_lock)
        {
            _backgroundServices.Add(service);
        }
    }

    public async Task UnregisterServiceAync(long taskId)
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
            await _backgroundServiceService.DeleteAsync(service);
        }
    }

    public async Task UpdateServiceAsync(DbBackgroundService service)
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
                await _backgroundServiceService.UpdateAsync(service);
            }
        }
    }
}
