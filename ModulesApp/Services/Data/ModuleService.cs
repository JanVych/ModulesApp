using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class ModuleService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public event Action? ModulesDbChangedEvent;

    public ModuleService(IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    private int SaveChanges(SQLiteDb context)
    {
        var result = context.SaveChanges();
        ModulesDbChangedEvent?.Invoke();
        return result;
    }

    private async Task<int> SaveChangesAsync(SQLiteDb context)
    {
        var result = await context.SaveChangesAsync();
        ModulesDbChangedEvent?.Invoke();
        return result;
    }

    public void Add(DbModule module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Add(module);
        SaveChanges(context);
    }

    public void Update(DbModule module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Update(module);
        SaveChanges(context);
    }

    public async Task UpdateAsync(DbModule module)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Modules.Update(module);
        await SaveChangesAsync(context);
    }

    public void Delete(DbModule module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Remove(module);
        SaveChanges(context);
    }

    public async Task<List<DbModule>> GetAllAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Modules
            .Include(x => x.Actions)
            .ToListAsync();
    }

    public List<DbModule> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules
            .ToList();
    }

    public DbModule? Get(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules
            .FirstOrDefault(x => x.Id == id);
    }

    public async Task<DbModule?> GetAsyncIncludeAll(long id)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Modules
            .Include(x => x.Actions)
            .Include(x => x.ServerTasks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public bool IsRegistrated(long id, string key)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules.Any(x => x.Id == id && x.Key == key);
    }

    public bool Exist(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules.Any(x => x.Id == id);
    }
}
