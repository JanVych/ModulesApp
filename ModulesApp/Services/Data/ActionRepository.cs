using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class ActionRepository
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ActionRepository(IDbContextFactory<SQLiteDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task SendToModuleAsync(long moduleId, string key, object? value)
    {
        await AddOrReplaceAsync(key, value, moduleId, null);
    }

    public async Task SendToBackgroundServiceAsync(long backgroundServiceId, string key, object? value)
    {
        await AddOrReplaceAsync(key, value, null, backgroundServiceId);
    }

    public async Task AddOrReplaceAsync(string key, object? value, long? moduleId, long? backgroundServiceId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        if (moduleId == null && backgroundServiceId == null) return;

        if (moduleId != null && !await context.Modules.AnyAsync(x => x.Id == moduleId)) return;

        if (backgroundServiceId != null && !await context.BackgroundServices.AnyAsync(x => x.Id == backgroundServiceId)) return;


        await context.Actions
            .Where(x => x.Key == key && x.ModuleId == moduleId && x.BackgroundServiceId == backgroundServiceId)
            .ExecuteDeleteAsync();

        context.Actions.Add(new DbAction
        {
            Key = key,
            Value = value,
            ModuleId = moduleId,
            BackgroundServiceId = backgroundServiceId
        });

        await context.SaveChangesAsync();
    }

    public async Task<List<DbAction>> GetListAndDeleteAsync(DbModule module)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
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
