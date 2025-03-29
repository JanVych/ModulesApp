using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.BackgroundServices;

namespace ModulesApp.Services.Data;

public class BackgroundServiceService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public event Action<DbBackgroundService>? BackgroundServiceChangedEvent;

    public BackgroundServiceService(IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public DbBackgroundService? Get(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices
            .FirstOrDefault(x => x.Id == id);
    }

    public async Task<List<DbBackgroundService>> GetListAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices
            .ToListAsync();
    }
}
