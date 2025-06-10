using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

[DisallowConcurrentExecution]
public abstract class BackgroundService: IJob
{
    protected readonly ContextService _contextService;

    protected IEnumerable<DbAction> Actions = [];
    protected Dictionary<string, object?> Data { get; set; } = [];

    public BackgroundService(ContextService contextService) 
    { 
        _contextService = contextService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        {
            if (_contextService != null)
            {
                _ = long.TryParse(context.JobDetail.Key.Name, out long id);

                var service = await _contextService.GetBackGroundServiceAsync(id) 
                    ?? throw new ArgumentNullException(id.ToString(), "Background service not found.");

                Actions = service.Actions;
                Data = service.Data;

                await ExecuteAsync(context);

                service.Data = Data;
                await _contextService.UpdateBackgroundServiceDataAsync(service);
                await _contextService.ExecuteServerTasksAsync(service);

            }
        }
    }

    public abstract Task ExecuteAsync(IJobExecutionContext context);
}
