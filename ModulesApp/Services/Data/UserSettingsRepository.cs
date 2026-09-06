using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class UserSettingsRepository
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public UserSettingsRepository(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Returns the UserSettings row, creating a default one on first use if none exists yet.
    /// </summary>
    public async Task<DbUserSettings> GetAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var settings = await db.UserSettings.FirstOrDefaultAsync();
        if (settings is null)
        {
            settings = new DbUserSettings();
            db.UserSettings.Add(settings);
            await db.SaveChangesAsync();
        }

        return settings;
    }

    public async Task UpdateAsync(DbUserSettings settings)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.UserSettings.Update(settings);
        await db.SaveChangesAsync();
    }
}
