using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.Dasboards;
using System.Text.Json;

namespace ModulesApp.Services.Data;

public class DashboardRepository
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    private readonly NotifyService _notifyService;

    public DashboardRepository(IDbContextFactory<SQLiteDbContext> dbContextFactory, NotifyService notifyService)
    {
        _dbContextFactory = dbContextFactory;
        _notifyService = notifyService;
    }

    public async Task AddAsync(DbDashboard dashboard)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        await context.Dashboards.AddAsync(dashboard);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DbDashboard dashboard)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Dashboards.Remove(dashboard);
        await context.SaveChangesAsync();
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

    public async Task<JsonElement?> GetMessageDataAsync(long serviceId, string key)
    {
        var entity = await GetEntityAsync(serviceId);
        if (entity == null || entity.Data == null)
        {
            return null;
        }
        if (entity.Data.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    /// Entities

    public async Task AddAsync(DbDashboardEntity entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        entity.SaveToData();
        await context.DashboardEntities.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task SendToDashboardEntity(long entityId, string key, object? value)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var entity = await context.DashboardEntities.FindAsync(entityId);
        if (entity != null)
        {
            entity.UpdateState(key, value, true);
            context.Entry(entity).Property(e => e.Data).IsModified = true;

            await context.SaveChangesAsync();
            _notifyService.NotifyDashboardEntityDataChanged(entityId, key, value);
        }
    }

    public async Task UpdateAsync(DbDashboardEntity entity, string? key = null, object? value = null)
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

    public async Task UpdateAsync(List<DbDashboardEntity> entities)
    {
        await using var context = _dbContextFactory.CreateDbContext();

        foreach (var entity in entities)
        {
            entity.SaveToData();
            context.DashboardEntities.Update(entity);
        }
        await context.SaveChangesAsync();
    }

    public async Task DeleteyAsync(DbDashboardEntity entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.DashboardEntities.Remove(entity);
        await context.SaveChangesAsync();
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

    public async Task<DbDashboardEntity?> GetEntityAsync(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.DashboardEntities.FindAsync(id);
    }
}
