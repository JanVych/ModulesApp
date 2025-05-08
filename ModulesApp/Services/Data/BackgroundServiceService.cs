using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.BackgroundServices;

namespace ModulesApp.Services.Data;

public class BackgroundServiceService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    private readonly NotifyService _notifyService;

    public BackgroundServiceService(IDbContextFactory<SQLiteDbContext> dbContextFactory, NotifyService notifyService)
    {
        _dbContextFactory = dbContextFactory;
        _notifyService = notifyService;
    }

    private async Task SaveChangesAsync(SQLiteDbContext context)
    {
        await context.SaveChangesAsync();
        _notifyService.NotifyBackgroundServiceChanged();
    }

    public DbBackgroundService? Get(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices
            .FirstOrDefault(x => x.Id == id);
    }

    public async Task<DbBackgroundService?> GetAsync(long id)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices
            .Include(x => x.Actions)
            .Include(x => x.ServerTasks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<DbBackgroundService>> GetAllAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices
            .ToListAsync();
    }

    public List<DbBackgroundService> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices
            .ToList();
    }

    public async Task AddAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Add(service);
        await SaveChangesAsync(context);
    }

    public async Task UpdateAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Update(service);
        await SaveChangesAsync(context);
    }

    public async Task DeleteAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Remove(service);
        await SaveChangesAsync(context);
    }

    public bool Exist(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices
            .Any(x => x.Id == id);
    }
}
