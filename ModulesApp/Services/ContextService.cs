using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using ModulesApp.Services.Data;
using System.Text.Json;

namespace ModulesApp.Services;

public class ContextService
{
    private readonly ModuleService _modulesService;
    public readonly DashboardService _dashboardService;
    private readonly ActionService _moduleActionService;
    public readonly BackgroundServiceService _backgroundServiceService;
    private readonly ServerTaskService _serverTaskService;


    public ContextService(ModuleService moduleService, DashboardService dashboardService, ActionService moduleActionService, BackgroundServiceService backgroundServiceService, ServerTaskService serverTaskService)
    {
        _modulesService = moduleService;
        _dashboardService = dashboardService;
        _moduleActionService = moduleActionService;
        _backgroundServiceService = backgroundServiceService;
        _serverTaskService = serverTaskService;
    }

    public List<DbDashboardEntity> GetAllDashBoardEntities()
    {
        return _dashboardService.GetAllDashBoardEntities();
    }

    public async Task<List<DbDashboardEntity>> GetAllDashBoardEntitiesAsync()
    {
        return await _dashboardService.GetAllDashBoardEntitiesAsync();
    }

    public List<DbBackgroundService> GetAllBackgroundServices()
    {
        return _backgroundServiceService.GetAll();
    }


    public async Task<List<DbBackgroundService>> GetAllBackgroundServicesAsync()
    {
        return await _backgroundServiceService.GetAllAsync();
    }

    public List<DbModule> GetAllModules()
    {
        return _modulesService.GetAll();
    }

    public async Task<List<DbModule>> GetAllModulesAsync()
    {
        return await _modulesService.GetAllAsync();
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
        if (service == null || service.MessageData == null)
        {
            return null;
        }
        if (service.MessageData.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return element;
        }
        return null;
    }

    public JsonElement? GetMessageFromDashBoardEntity(long serviceId, string key)
    {
        var entity = _dashboardService.GetEntity(serviceId);
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

    public void SendToModule(long moduleId, string key, object? value)
    {
        if (_modulesService.Exist(moduleId))
        {
            _moduleActionService.AddOrReplace(key, value, moduleId, null);
            //_moduleActionService.Add(action);
        }
    }

    public void SendToBackgroundService(long serviceId, string key, object? value)
    {
        if (_backgroundServiceService.Exist(serviceId))
        {
            _moduleActionService.AddOrReplace(key, value, null, serviceId);
            //_moduleActionService.Add(action);
        }
    }


    public void SendToDashboardEntity(long entityId, string key, object? value)
    {
        _dashboardService.UpdateEntity(entityId, key, value);
    }

    public async Task DashboardEntityUserTriggerAsync(DbDashboardEntity entity)
    {
        _dashboardService.UpdateEntity(entity);
        await _serverTaskService.ExecuteTasksAsync(this, entity);
    }

    public void UpdateFromBackgroundService(DbBackgroundService service)
    {
        _backgroundServiceService.UpdateFromBackgroundService(service);
    }

    public async Task ExecuteServerTasksAsync(DbBackgroundService service)
    {
        await _serverTaskService.ExecuteTasksAsync(this, service);
    }
}
