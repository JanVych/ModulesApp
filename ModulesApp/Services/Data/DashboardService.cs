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
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var dashboards = await context.Dashboards
            .Include(d => d.Entities)
            .ThenInclude(e => e.ChildEntities)
            .ToListAsync();

        dashboards.AsParallel().ForAll(d => d.Entities.ForEach(e => e.LoadState()));

        return dashboards;
    }

    /// Entities

    public void UpdateEntityAndNotify(long entityId, string key, object? value)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var entity = context.DashboardEntities.Find(entityId);

        if (entity != null)
        {
            entity.UpdateState(key, value, true);
            context.Entry(entity).Property(e => e.Data).IsModified = true;

            context.SaveChanges();
            _notifyService.NotifyDashboardEntityDataChanged(entityId, key, value);
        }
    }

    public void AddEntity(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveToData();
        context.DashboardEntities.Add(entity);
        context.SaveChanges();
    }

    public async Task UpdateEntityAsync(DbDashboardEntity entity, string? key = null, object? value = null)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        entity.SaveToData();
        context.DashboardEntities.Attach(entity);
        context.Entry(entity).Property(e => e.Data).IsModified = true;

        await context.SaveChangesAsync();

        if (key != null)
        {
            _notifyService.NotifyDashboardEntityDataChanged(entity.Id, key, value);
        }
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
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.DashboardEntities
            .ToListAsync();
    }

    public DbDashboardEntity? GetEntity(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var entity = context.DashboardEntities
            .FirstOrDefault(x => x.Id == id);
        return entity;
    }
}
