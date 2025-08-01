using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.ModulesPrograms;

namespace ModulesApp.Services.Data;

public class ModuleProgramService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ModuleProgramService(IDbContextFactory<SQLiteDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public List<DbModuleProgram> GetProgramsList()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Programs.ToList();
    }

    public async Task<List<DbModuleProgram>> GetProgramsListAsync()
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Programs
            .Include(p => p.Files)
            .Include(p => p.Firmware)
            .ToListAsync();
    }

    public async Task<List<DbModuleFirmware>> GetFirmwareListAsync()
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Firmwares
            .ToListAsync();
    }

    public async Task<DbModuleProgram> AddAsync(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Add(program);
        await db.SaveChangesAsync();
        return program;
    }

    public async Task UpdateAsync(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Update(program);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Remove(program);
        await db.SaveChangesAsync();
    }

    public async Task AddAsync(DbModuleFirmware firmware)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Firmwares.Add(firmware);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(DbModuleFirmware firmware)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Firmwares.Remove(firmware);
        await db.SaveChangesAsync();
    }
}
