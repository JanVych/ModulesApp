using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class ModuleActionService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public ModuleActionService(IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public void Add(DbModuleAction moduleAction)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.ModuleActions.Add(moduleAction);
        context.SaveChanges();
    }

    public async Task DeleteAsync(IEnumerable<DbModuleAction> actions)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.ModuleActions.RemoveRange(actions);
        await context.SaveChangesAsync();
    }

    public async Task<List<DbModuleAction>> GetListAsync(long moduleId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.ModuleActions
            .Where(x => x.ModuleId == moduleId)
            .ToListAsync();
    }

    public async Task<List<DbModuleAction>> GetListAndDeleteAsync(long moduleId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var actions = await context.ModuleActions
            .Where(x => x.ModuleId == moduleId)
            .AsNoTracking()
            .ToListAsync();
        
        await context.ModuleActions
            .Where(x => x.ModuleId == moduleId)
            .ExecuteDeleteAsync();
        return actions;
    }
}
