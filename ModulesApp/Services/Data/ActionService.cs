using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class ActionService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ActionService(IDbContextFactory<SQLiteDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public void AddOrReplace(string key, object? value, long? moduleId, long? backgroundServiceId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var existingActions = context.Actions.Where(x => x.Key == key && x.ModuleId == moduleId && x.BackgroundServiceId == backgroundServiceId);
        context.Actions.RemoveRange(existingActions);
        var newAction = new DbAction
        {
            Key = key,
            Value = value,
            ModuleId = moduleId,
            BackgroundServiceId = backgroundServiceId
        };
        context.Actions.Add(newAction);
        context.SaveChanges();
    }

    public async Task AddOrReplaceAsync(string key, object? value, long? moduleId, long? backgroundServiceId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        await context.Actions
            .Where(x => x.Key == key && x.ModuleId == moduleId && x.BackgroundServiceId == backgroundServiceId)
            .ExecuteDeleteAsync();

        var newAction = new DbAction
        {
            Key = key,
            Value = value,
            ModuleId = moduleId,
            BackgroundServiceId = backgroundServiceId
        };

        context.Actions.Add(newAction);
        await context.SaveChangesAsync();
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
