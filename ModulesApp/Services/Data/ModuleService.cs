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

    public void Add(Module module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Add(module);
        SaveChanges(context);
    }

    public void Update(Module module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Update(module);
        SaveChanges(context);
    }

    public void Delete(Module module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Modules.Remove(module);
        SaveChanges(context);
    }

    public async Task<List<Module>> GetAllAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Modules
            .Include(x => x.ModuleActions)
            .ToListAsync();
    }

    public List<Module> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules
            .ToList();
    }

    public Module? Get(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules
            .FirstOrDefault(x => x.Id == id);
    }

    public bool IsRegistrated(long id, string key)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules.Any(x => x.Id == id && x.Key == key);
    }

    public bool IsRegistrated(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Modules.Any(x => x.Id == id);
    }
}
