using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models.ModulesPrograms;

namespace ModulesApp.Services.Data;

public class ModuleProgramService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public ModuleProgramService(IDbContextFactory<SQLiteDb> dbContextFactory)
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
            .ThenInclude(f => f.IDF)
            .ToListAsync();
    }

    public async Task<List<DbModuleIDF>> GetIDFListAsync()
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.IDFs
            .ToListAsync();
    }

    public async Task<List<DbModuleFirmware>> GetFirmwareListAsync()
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Firmwares
            .Include(f => f.IDF)
            .ToListAsync();
    }

    public async Task<DbModuleProgram> Add(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Add(program);
        await db.SaveChangesAsync();
        return program;
    }

    public async Task Update(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Update(program);
        await db.SaveChangesAsync();
    }

    public async Task Delete(DbModuleProgram program)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();
        db.Programs.Remove(program);
        await db.SaveChangesAsync();
    }
}
