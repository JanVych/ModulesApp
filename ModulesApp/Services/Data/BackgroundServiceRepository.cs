using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.BackgroundServices;
using System.Text.Json;

namespace ModulesApp.Services.Data;

public class BackgroundServiceRepository
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly NotifyService _notifyService;

    public BackgroundServiceRepository(IDbContextFactory<AppDbContext> dbContextFactory, NotifyService notifyService)
    {
        _dbContextFactory = dbContextFactory;
        _notifyService = notifyService;
    }

    private async Task SaveChangesAsync(AppDbContext context)
    {
        await context.SaveChangesAsync();
        _notifyService.NotifyBackgroundServiceChanged();
    }

    public async Task<JsonElement?> GetMessageDataAsync(long serviceId, string key)
    {
        var service = await GetAsync(serviceId);
        if (service == null || service.MessageData == null)
        {
            return null;
        }
        if (service.MessageData.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    public async Task<DbBackgroundService?> GetAsync(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices
            .Include(x => x.Actions)
            .Include(x => x.ServerTasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DbBackgroundService?> GetAndDeleteActionsAsync(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var services = await context.BackgroundServices
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == id);

        await context.Actions
            .Where(x => x.BackgroundServiceId == id)
            .ExecuteDeleteAsync();
        return services;
    }

    public async Task<List<DbBackgroundService>> GetAllAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.BackgroundServices.ToListAsync();
    }

    public List<DbBackgroundService> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.BackgroundServices.ToList();
    }

    public async Task AddAsync(DbBackgroundService service)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Add(service);
        await SaveChangesAsync(context);
    }

    public async Task UpdateAsync(DbBackgroundService service)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Entry(service).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task UpdateFromBackgroundServiceAsync(DbBackgroundService service)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var existingService = await context.BackgroundServices.FindAsync(service.Id);
        if (existingService != null)
        {
            existingService.MessageData = service.MessageData;
            existingService.ConfigurationData = service.ConfigurationData;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.BackgroundServices.Remove(service);
        await SaveChangesAsync(context);
    }
}
