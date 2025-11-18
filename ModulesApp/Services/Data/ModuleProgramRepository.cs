using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.ModulesPrograms;

namespace ModulesApp.Services.Data;

public class ModuleProgramRepository
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ModuleProgramRepository(IDbContextFactory<SQLiteDbContext> dbContextFactory)
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
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Programs
            .Include(p => p.Files)
            .Include(p => p.Firmware)
            .ToListAsync();
    }

    public async Task<List<DbModuleFirmware>> GetFirmwareListAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Firmwares
            .ToListAsync();
    }

    public async Task<DbModuleProgram> AddAsync(DbModuleProgram program)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Add(program);
        await db.SaveChangesAsync();
        return program;
    }

    public async Task UpdateAsync(DbModuleProgram program)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Update(program);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(DbModuleProgram program)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Remove(program);
        await db.SaveChangesAsync();
    }

    public async Task AddAsync(DbModuleFirmware firmware)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Firmwares.Add(firmware);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(DbModuleFirmware firmware)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Firmwares.Remove(firmware);
        await db.SaveChangesAsync();
    }
}
