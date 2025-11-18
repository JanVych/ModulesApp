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


    public ContextService(ModuleRepository moduleService, DashboardRepository dashboardService, ActionRepository moduleActionService, BackgroundServiceRepository backgroundServiceService, ServerTaskRepository serverTaskService)
    {
        ModuleRepository = moduleService;
        DashboardRepository = dashboardService;
        ActionRepository = moduleActionService;
        BackgroundServiceRepository = backgroundServiceService;
        ServerTaskRepository = serverTaskService;
    }

    public async Task DashboardEntityUserTriggerAsync(DbDashboardEntity entity, string? key=null, object? value=null)
    {
        await DashboardRepository.UpdateAsync(entity, key, value);
        await ServerTaskRepository.ExecuteTasksAsync(this, entity);
    }
}
