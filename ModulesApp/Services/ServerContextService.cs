using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.Dasboards;
using ModulesApp.Services.Data;
using System.Diagnostics;
using System.Text.Json;

namespace ModulesApp.Services;

public class ServerContextService : IServerContext
{
    private readonly ModuleService _modulessService;
    private readonly DashboardService _dashboardService;
    private readonly ModuleActionService _moduleActionService;

    public ServerContextService(ModuleService moduleService, DashboardService dashboardService, ModuleActionService moduleActionService)
    {
        _modulessService = moduleService;
        _dashboardService = dashboardService;
        _moduleActionService = moduleActionService;
    }

    public void DisplayValue(long dashboardEntityId, Dictionary<string, object> data)
    {
        _dashboardService.EntityDataChanged(dashboardEntityId, data);
    }

    public List<DbDashboardEntity> GetAllDashBoardEntities()
    {
        return _dashboardService.GetAllDashBoardEntities();
    }


    public List<DbModule> GetAllModules()
    {
        return _modulessService.GetAll();
    }

    public JsonElement? GetMessageFromModule(long moduleId, string key)
    {
        var module = _modulessService.Get(moduleId);
        if (module == null || module.JsonData == null)
        {
            return null;
        }

        if (module.JsonData.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    public void SendToModule(long moduleId, string key, object value)
    {
        if (_modulessService.IsRegistrated(moduleId))
        {
            var action = new DbModuleAction
            {
                ModuleId = moduleId,
                Key = key,
                Value = value
            };
            _moduleActionService.Add(action);
        }
    }
}
