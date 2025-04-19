using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
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
    private readonly ServerTaskService _serverTaskService;


    public ServerContextService(ModuleService moduleService, DashboardService dashboardService, ActionService moduleActionService, BackgroundServiceService backgroundServiceService, ServerTaskService serverTaskService)
    {
        _modulesService = moduleService;
        _dashboardService = dashboardService;
        _moduleActionService = moduleActionService;
        _backgroundServiceService = backgroundServiceService;
        _serverTaskService = serverTaskService;
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

    public async Task<List<DbModule>> GetAllModulesAsync()
    {
        return await _modulesService.GetAllAsync();
    }

    public async Task<List<DbAction>> GetActionsAsync(DbBackgroundService service)
    {
        return await _moduleActionService.GetListAndDeleteAsync(service);
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

    public JsonElement? GetMessageFromDashBoardEntity(long serviceId, string key)
    {
        var entity = _dashboardService.GetDashBoardEntity(serviceId);
        if (entity == null || entity.Data == null)
        {
            return null;
        }
        if (entity.Data.TryGetValue(key, out var value) && value is JsonElement element)
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

    public async Task DashboardEntityUserTrigger(DbDashboardEntity entity)
    {
        _dashboardService.Update(entity);
        await _serverTaskService.ExecuteTasksAsync(this, entity);
    }

    public async Task UpdateFromBackgroundService(DbBackgroundService service)
    {
        await _backgroundServiceService.UpdateAsync(service);
    }

    public async Task ExecuteServerTasksAsync(DbBackgroundService service)
    {
        await _serverTaskService.ExecuteTasksAsync(this, service);
    }
}
