using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;

namespace ModulesApp.Services.Data;

public class ActionService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ActionService(IDbContextFactory<SQLiteDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public void Add(DbAction moduleAction)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Actions.Add(moduleAction);
        context.SaveChanges();
    }

    public async Task DeleteAsync(IEnumerable<DbAction> actions)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Actions.RemoveRange(actions);
        await context.SaveChangesAsync();
    }

    public async Task<List<DbAction>> GetListAndDeleteAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var actions = await context.Actions
            .Where(x => x.BackgroundServiceId == service.Id)
            .AsNoTracking()
            .ToListAsync();

        await context.Actions
            .Where(x => x.BackgroundServiceId == service.Id)
            .ExecuteDeleteAsync();
        return actions;
    }

    public async Task<List<DbAction>> GetListAndDeleteAsync(DbModule module)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var actions = await context.Actions
            .Where(x => x.ModuleId == module.Id)
            .AsNoTracking()
            .ToListAsync();
        
        await context.Actions
            .Where(x => x.ModuleId == module.Id)
            .ExecuteDeleteAsync();
        return actions;
    }
}
