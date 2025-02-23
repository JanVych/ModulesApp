using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.BackgroundService;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ModulesApp.Services.Background;

public class BackgroundServiceManager
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    private readonly ILogger<BackgroundServiceManager> _logger;

    private readonly ServerContextService _serverContextService;

    private readonly ConcurrentBag<DbBackgroundService> _backgroundServices;

    public BackgroundServiceManager(IDbContextFactory<SQLiteDb> dbContextFactory, ServerContextService serverContextService, ILogger<BackgroundServiceManager> logger)
    {
        _dbContextFactory = dbContextFactory;
        _serverContextService = serverContextService;
        _logger = logger;

        using var db = dbContextFactory.CreateDbContext();
        _backgroundServices = new ConcurrentBag<DbBackgroundService>(db.BackgroundServices);
        Launch();
    }

    private void Launch()
    {
        foreach (var backgroundService in _backgroundServices)
        {
            if(backgroundService.IsRunning)
            {
                Start(backgroundService.Id);
            }
        }
    }

    public void Start(long taskId)
    {
        var backgroundService = _backgroundServices.FirstOrDefault(x => x.Id == taskId);
        if (backgroundService != null)
        {
            Task.Run(() => backgroundService.StartAsync(_serverContextService));
        }
    }

    public async Task Stop(long taskId)
    {
        var backgroundService = _backgroundServices.FirstOrDefault(x => x.Id == taskId);
        if (backgroundService != null)
        {
            await backgroundService.StopAsync();
        }
    }

    public async Task Add(DbBackgroundService backgroundService)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.BackgroundServices.Add(backgroundService);
        await db.SaveChangesAsync();
        _backgroundServices.Add(backgroundService);
    }

    public async Task Remove(long taskId)
    {
        var backgroundService = _backgroundServices.FirstOrDefault(x => x.Id == taskId);
        if (backgroundService != null)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();
            db.BackgroundServices.Remove(backgroundService);
            await db.SaveChangesAsync();
            _backgroundServices.TryTake(out backgroundService);
        }
    }

    public List<DbBackgroundService> GetAll()
    {
        return _backgroundServices.ToList();
    }
}
