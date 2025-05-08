using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.Dasboards;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public void UpdateDashboard(DbDashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Update(dashboard);
        context.SaveChanges();
    }

    public void DeleteDashboard(DbDashboard dashboard)
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

        dashboards.AsParallel().ForAll(d => d.Entities.ForEach(e => e.UpdateFromData()));

        return dashboards;
    }

    /// Entities

    public void UpdateEntity(long entityId, string key, object? value)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var entity = context.DashboardEntities.FirstOrDefault(x => x.Id == entityId);
        if (entity != null)
        {
            entity.Data[key] = value;
            context.DashboardEntities.Update(entity);
            context.SaveChanges();
            context.Entry(entity).Reload();
            entity.UpdateFromData();
            _notifyService.NotifyDashboardEntityDataChanged(entity);
        }
    }

    public void AddEntity(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveData();
        context.DashboardEntities.Add(entity);
        context.SaveChanges();
    }

    public void UpdateEntity(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        entity.SaveData();
        context.DashboardEntities.Update(entity);
        context.SaveChanges();
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
            .Include(x => x.Dashboard)
            .FirstOrDefault(x => x.Id == id);
    }
}
