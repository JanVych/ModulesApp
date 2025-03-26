//using Microsoft.EntityFrameworkCore;
//using ModulesApp.Data;
//using ModulesApp.Models.BackgroundServices;

//namespace ModulesApp.Services.Data;

//public class BackgroundServiceService
//{
//    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

//    public event Action<DbBackgroundService>? BackgroundServiceChangedEvent;

//    public BackgroundServiceService(IDbContextFactory<SQLiteDb> dbContextFactory)
//    {
//        _dbContextFactory = dbContextFactory;
//    }

//    public List<DbBackgroundService> GetList()
//    {
//        using var context = _dbContextFactory.CreateDbContext();
//        return context.BackgroundServices.ToList();
//    }

//    public async Task<List<DbBackgroundService>> GetListAsync()
//    {
//        using var context = await _dbContextFactory.CreateDbContextAsync();
//        return await context.BackgroundServices.ToListAsync();
//    }

//    public async Task<DbBackgroundService?> GetAsync(long id)
//    {
//        using var context = await _dbContextFactory.CreateDbContextAsync();
//        return await context.BackgroundServices.FirstOrDefaultAsync(x => x.Id == id);
//    }

//    public async Task Update(DbBackgroundService backgroundService)
//    {
//        using var context = await _dbContextFactory.CreateDbContextAsync();
//        context.BackgroundServices.Update(backgroundService);
//        await context.SaveChangesAsync();
//        BackgroundServiceChangedEvent?.Invoke(backgroundService);
//    }
//}
