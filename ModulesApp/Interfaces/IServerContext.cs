using ModulesApp.Models;
using System.Diagnostics;
using System.Text.Json;

namespace ModulesApp.Interfaces;

public interface IServerContext
{
    public void SendToModule(long moduleId, string key, object jsonElement);
    public JsonElement? GetMessageFromModule(long moduleId, string messageKey);
    public void DisplayValue(long boardCardId, string name, string value);

    public List<Module> GetAllModules();
    public List<DashBoardCard> GetAllBoardCards();
}

public class FakeContext : IServerContext
{
    public void SendToModule(long moduleId, string key, object value)
    {
        Debug.WriteLine($"Sending to module {moduleId}, {key}, {value}");
    }
    public JsonElement? GetMessageFromModule(long moduleId, string key)
    {
        return new JsonElement();
    }
    public List<Module> GetAllModules()
    {
        var list = new List<Module>
        {
            new() { Id = 1, Name = "Module 1" },
            new() { Id = 2, Name = "Module 2" },
            new() { Id = 3, Name = "Module 3" }
        };
        return list;

    }
    public List<DashBoardCard> GetAllBoardCards()
    {
        var List = new List<DashBoardCard>
        {
            new() { Id = 1, Name = "Card 1" },
            new() { Id = 2, Name = "Card 2" },
            new() { Id = 3, Name = "Card 3" }
        };
        return List;
    }

    public void DisplayValue(long boardCardId, string name, string value)
    {
        Debug.WriteLine($"Displaying, name {name}, value {value} on board card {boardCardId}");
    }
}
