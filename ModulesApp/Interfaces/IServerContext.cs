using ModulesApp.Models;
using ModulesApp.Models.Dasboards;
using System.Text.Json;

namespace ModulesApp.Interfaces;

public interface IServerContext
{
    public void SendToModule(long moduleId, string key, object value);
    public JsonElement? GetMessageFromModule(long moduleId, string messageKey);
    public JsonElement? GetMessageFromService(long serviceId, string messageKey);
    public JsonElement? GetMessageFromDashBoardEntity(long entityId, string messageKey);

    public void DisplayValue(long dashboardEntityId, Dictionary<string, object> data);

    public List<DbModule> GetAllModules();
    public List<DbDashboardEntity> GetAllDashBoardEntities();
}

//public class FakeContext : IServerContext
//{
//    public void SendToModule(long moduleId, string key, object value)
//    {
//        Debug.WriteLine($"Sending to module {moduleId}, {key}, {value}");
//    }
//    public JsonElement? GetMessageFromModule(long moduleId, string key)
//    {
//        return new JsonElement();
//    }
//    public List<DbModule> GetAllModules()
//    {
//        var list = new List<DbModule>
//        {
//            new() { Id = 1, Name = "Module 1" },
//            new() { Id = 2, Name = "Module 2" },
//            new() { Id = 3, Name = "Module 3" }
//        };
//        return list;

//    }
//    public List<DbBasicCardEntity> GetAllDashBoardEntities()
//    {
//        var List = new List<DbBasicCardEntity>
//        {
//            new() { Id = 1 },
//            new() { Id = 2 },
//            new() { Id = 3 }
//        };
//        return List;
//    }

//    public void DisplayValue(long boardCardId, string name, string value)
//    {
//        Debug.WriteLine($"Displaying, name {name}, value {value} on board card {boardCardId}");
//    }
//}
