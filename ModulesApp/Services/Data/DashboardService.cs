using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.Dasboards;

namespace ModulesApp.Services.Data;

public class DashboardService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public event Action<long, Dictionary<string, object>>? DashboardEntityDataEvent;

    public DashboardService(IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

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

    public async Task<List<DbDashboard>> GetAllDashboardsAync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Dashboards
            .Include(x => x.Entities)
            .ToListAsync();
    }

    /// Entities
    
    public void EntityDataChanged(long entityId, Dictionary<string, object> data)
    {
        Update(entityId, data);
        DashboardEntityDataEvent?.Invoke(entityId, data);
    }

    public void Add(DbDashboardEntity entity)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.DashboardEntities.Add(entity);
        context.SaveChanges();
    }

    public void Update(long entityId, Dictionary<string, object> data)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var card = context.DashboardEntities.FirstOrDefault(x => x.Id == entityId);
        if (card != null)
        {
            card.Data = data;
            context.DashboardEntities.Update(card);
            context.SaveChanges();
        }
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
}
