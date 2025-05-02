using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.Dasboards;

namespace ModulesApp.Services.Data;

public class DashboardService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    private readonly NotifyService _notifyService;

    public DashboardService(IDbContextFactory<SQLiteDbContext> dbContextFactory, NotifyService notifyService)
    {
        _dbContextFactory = dbContextFactory;
        _notifyService = notifyService;
    }

    public void Add(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Add(dashboard);
        context.SaveChanges();
    }

    public void Update(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Update(dashboard);
        context.SaveChanges();
    }

    public void Delete(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Remove(dashboard);
        context.SaveChanges();
    }

    public DbDashboard? GetDashBoard(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Dashboards
            .Include(x => x.Entities)
            .FirstOrDefault(x => x.Id == id);
    }
    public async Task<List<DbDashboard>> GetAllDashboardsAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var dashboards = await context.Dashboards
            .Include(d => d.Entities)
            .ToListAsync();

        dashboards.AsParallel().ForAll(d => d.Entities.ForEach(e => e.UpdateData()));

        return dashboards;
    }

    /// Entities

    public void EntityDataChanged(long entityId, Dictionary<string, object?> data)
    {
        var entity = UpdateFromTaskAsync(entityId, data);
        if (entity != null)
        {
            _notifyService.NotifyDashboardEntityDataChanged(entity);
        }
    }

    public void Add(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveData();
        context.DashboardEntities.Add(entity);
        context.SaveChanges();
    }

    public DbDashboardEntity? UpdateFromTaskAsync(long entityId, Dictionary<string, object?> data)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var entity = context.DashboardEntities.FirstOrDefault(x => x.Id == entityId);
        entity?.UpdateData(data);
        if (entity != null)
        {
            context.DashboardEntities.Update(entity);
            context.SaveChanges();
        }
        return entity;
    }

    public void Update(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveData();
        context.DashboardEntities.Update(entity);
        context.SaveChanges();
    }

    public void Delete(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.DashboardEntities.Remove(entity);
        context.SaveChanges();
    }

    public List<DbDashboardEntity> GetAllDashBoardEntities()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.DashboardEntities
            .ToList();
    }

    public async Task<List<DbDashboardEntity>> GetAllDashBoardEntitiesAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.DashboardEntities
            .ToListAsync();
    }

    public DbDashboardEntity? GetDashBoardEntity(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.DashboardEntities
            .Include(x => x.Dashboard)
            .FirstOrDefault(x => x.Id == id);
    }
}
