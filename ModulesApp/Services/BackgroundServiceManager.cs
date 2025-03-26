using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class BackgroundServiceManager
{
    //private readonly ILogger<BackgroundServiceManager> _logger;
    private readonly ServerTaskService _serverTaskService;
    private readonly ActionService _actionService;

    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;
    public event Action? BackgroundServiceChangedEvent;

    private readonly List<DbBackgroundService> _backgroundServices;
    private readonly object _lock = new();


    public BackgroundServiceManager( ServerTaskService serverTaskService, ActionService actionService, IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _serverTaskService = serverTaskService;
        _actionService = actionService;
        _dbContextFactory = dbContextFactory;   

        _backgroundServices = GetList();
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
                    Task.Run(() => backgroundService.StartAsync(this));
                }
                else
                {
                    backgroundService.Status = BackgroundServiceStatus.Stopped;
                }
            }
        }
    }

    public void Start(long serviceId)
    {
        DbBackgroundService? service;
        lock (_lock)
        {
            service = _backgroundServices.FirstOrDefault(x => x.Id == serviceId);
        }
        if (service is not null && service.Status == BackgroundServiceStatus.Stopped)
        {
            Task.Run(() => service.StartAsync(this));
        }
    }

    public async Task Stop(long serviceId)
    {
        DbBackgroundService? service;
        lock (_lock)
        {
            service = _backgroundServices.FirstOrDefault(x => x.Id == serviceId);
        }
        if (service is not null && service.Status == BackgroundServiceStatus.Running)
        {
            await service.StopAsync(this);
        }
    }

    public async Task<List<DbAction>> GetActions(DbBackgroundService service)
    {
        return await _actionService.GetListAndDeleteAsync(service);
    }

    public List<DbBackgroundService> GetList()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices.ToList();
    }

    public async Task<List<DbBackgroundService>> GetListAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices.ToListAsync();
    }

    public async Task<DbBackgroundService?> GetAsync(long id)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices
            .Include(x => x.Actions)
            .Include(x => x.ServerTasks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateFromService(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Update(service);
        await context.SaveChangesAsync();
        BackgroundServiceChangedEvent?.Invoke();
    }

    public async Task ExecuteServerTasks(DbBackgroundService service)
    {
        await _serverTaskService.ProcessNodes(service);
    }

    public async Task Add(DbBackgroundService service)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.BackgroundServices.Add(service);
        await db.SaveChangesAsync();
        lock (_lock)
        {
            _backgroundServices.Add(service);
        }
        BackgroundServiceChangedEvent?.Invoke();
    }

    public async Task Delete(long taskId)
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
            using var db = await _dbContextFactory.CreateDbContextAsync();
            db.BackgroundServices.Remove(service);
            await db.SaveChangesAsync();
            BackgroundServiceChangedEvent?.Invoke();
        }
    }

    public async Task Update(DbBackgroundService service)
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
                using var db = await _dbContextFactory.CreateDbContextAsync();
                db.BackgroundServices.Update(service);
                await db.SaveChangesAsync();
            }
        }
    }
}
