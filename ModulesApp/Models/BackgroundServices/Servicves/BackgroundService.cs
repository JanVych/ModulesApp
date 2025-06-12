using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

[DisallowConcurrentExecution]
public abstract class BackgroundService: IJob
{
    protected readonly ContextService _context;

    protected IEnumerable<DbAction> Actions = [];
    protected Dictionary<string, object?> MessageData { get; set; } = [];
    protected Dictionary<string, object?> ConfigurationData { get; set; } = [];

    public BackgroundService(ContextService contextService) 
    { 
        _context = contextService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        {
            if (_context != null)
            {
                _ = long.TryParse(context.JobDetail.Key.Name, out long id);

                var service = await _context._backgroundServiceService.GetAndDeleteActionsAsync(id) 
                    ?? throw new ArgumentNullException(id.ToString(), "Background service not found.");

                Actions = service.Actions;
                ConfigurationData = service.ConfigurationData;
                await ExecuteAsync(context);

                service.MessageData = MessageData;
                _context.UpdateFromBackgroundService(service);
                await _context.ExecuteServerTasksAsync(service);
            }
        }
    }

    public abstract Task ExecuteAsync(IJobExecutionContext context);
}
