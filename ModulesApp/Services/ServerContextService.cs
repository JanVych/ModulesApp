using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.Dasboards;
using ModulesApp.Services.Data;
using System.Text.Json;

namespace ModulesApp.Services;

public class ServerContextService : IServerContext
{
    private readonly ModuleService _modulesService;
    private readonly DashboardService _dashboardService;
    private readonly ActionService _moduleActionService;
    private readonly BackgroundServiceService _backgroundServiceService;


    public ServerContextService(ModuleService moduleService, DashboardService dashboardService, ActionService moduleActionService, BackgroundServiceService backgroundServiceService)
    {
        _modulesService = moduleService;
        _dashboardService = dashboardService;
        _moduleActionService = moduleActionService;
        _backgroundServiceService = backgroundServiceService;
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
        return _modulesService.GetAll();
    }

    public JsonElement? GetMessageFromModule(long moduleId, string key)
    {
        var module = _modulesService.Get(moduleId);
        if (module == null || module.Data == null)
        {
            return null;
        }

        if (module.Data.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    public JsonElement? GetMessageFromService(long serviceId, string key)
    {
        var service = _backgroundServiceService.Get(serviceId);
        if (service == null || service.Data == null)
        {
            return null;
        }
        if (service.Data.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    public void SendToModule(long moduleId, string key, object value)
    {
        if (_modulesService.IsRegistrated(moduleId))
        {
            var action = new DbAction
            {
                ModuleId = moduleId,
                Key = key,
                Value = value
            };
            _moduleActionService.Add(action);
        }
    }
}
