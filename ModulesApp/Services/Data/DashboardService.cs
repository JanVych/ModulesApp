using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Models;

namespace ModulesApp.Services.Data;

public class DashboardService
{
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public event Action<long, string, string>? DashboardCardDataEvent;

    public DashboardService(IDbContextFactory<SQLiteDb> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

    }

    public void Add(Dashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Add(dashboard);
        context.SaveChanges();
    }

    public void Update(Dashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Update(dashboard);
        context.SaveChanges();
    }

    public void Delete(Dashboard dashboard)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Dashboards.Remove(dashboard);
        context.SaveChanges();
    }

    public Dashboard? GetDashBoard(long id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Dashboards
            .Include(x => x.Cards)
            .FirstOrDefault(x => x.Id == id);
    }

    public async Task<List<Dashboard>> GetAllDashboardsAync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Dashboards
            .Include(x => x.Cards)
            .ToListAsync();
    }

    /// Cards
    
    public void CardValueChanged(long boardCardId, string name, string value)
    {
        Update(boardCardId, name, value);
        DashboardCardDataEvent?.Invoke(boardCardId, name, value);
    }

    public void Add(DashBoardCard card)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.DashBoardCards.Add(card);
        context.SaveChanges();
    }

    public void Update(long boardCardId, string name, string value)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var card = context.DashBoardCards.FirstOrDefault(x => x.Id == boardCardId);
        if (card != null)
        {
            card.Name = name;
            card.Value = value;
            context.DashBoardCards.Update(card);
            context.SaveChanges();
        }
    }

    public void Delete(DashBoardCard card)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.DashBoardCards.Remove(card);
        context.SaveChanges();
    }

    public List<DashBoardCard> GetAllDashBoardCards()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.DashBoardCards
            .ToList();
    }
}
