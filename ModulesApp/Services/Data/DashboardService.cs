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

    public void AddDashBoard(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Add(dashboard);
        context.SaveChanges();
    }

    public void DeleteDashboard(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Remove(dashboard);
        context.SaveChanges();
    }

    public async Task<List<DbDashboard>> GetAllDashboardsAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var dashboards = await context.Dashboards
            .Include(d => d.Entities)
            .ThenInclude(e => e.ChildEntities)
            .ToListAsync();

        dashboards.AsParallel().ForAll(d => d.Entities.ForEach(e => e.LoadState()));

        return dashboards;
    }

    /// Entities

    public void UpdateEntity(long entityId, string key, object? value)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var entity = context.DashboardEntities
            .Include(e => e.ChildEntities)
            .FirstOrDefault(x => x.Id == entityId);
        if (entity != null)
        {
            _notifyService.NotifyDashboardEntityDataChanged(entityId, key, value);
            entity.UpdateState(key, value, true);
            context.DashboardEntities.Update(entity);
            context.SaveChanges();
        }
    }

    public void AddEntity(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveToData();
        context.DashboardEntities.Add(entity);
        context.SaveChanges();
    }

    public void UpdateEntity(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveToData();
        context.DashboardEntities.Update(entity);
        context.SaveChanges();
    }

    public async Task UpdateEntitiesAsync(List<DbDashboardEntity> entities)
    {
        await using var context = _dbContextFactory.CreateDbContext();

        foreach (var entity in entities)
        {
            entity.SaveToData();
            context.DashboardEntities.Update(entity);
        }

        await context.SaveChangesAsync();
    }


    public void DeleteEntity(DbDashboardEntity entity)
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

    public DbDashboardEntity? GetEntity(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.DashboardEntities
            .FirstOrDefault(x => x.Id == id);
    }
}
