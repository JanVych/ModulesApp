using ModulesApp.Models.Dasboards;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class ContextService
{
    public readonly ModuleRepository ModuleRepository;
    public readonly DashboardRepository DashboardRepository;
    public readonly ActionRepository ActionRepository;
    public readonly BackgroundServiceRepository BackgroundServiceRepository;
    public readonly ServerTaskRepository ServerTaskRepository;
    public readonly UserSettingsRepository UserSettingsRepository;


    public ContextService(ModuleRepository moduleService, DashboardRepository dashboardService, 
        ActionRepository moduleActionService, BackgroundServiceRepository backgroundServiceService, 
        ServerTaskRepository serverTaskService, UserSettingsRepository userSettingsService)
    {
        ModuleRepository = moduleService;
        DashboardRepository = dashboardService;
        ActionRepository = moduleActionService;
        BackgroundServiceRepository = backgroundServiceService;
        ServerTaskRepository = serverTaskService;
        UserSettingsRepository = userSettingsService;
    }

    public async Task DashboardEntityUserTriggerAsync(DbDashboardEntity entity, string? key=null, object? value=null)
    {
        await DashboardRepository.UpdateAsync(entity, key, value);
        await ServerTaskRepository.ExecuteTasksAsync(this, entity);
    }
}
