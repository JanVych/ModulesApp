using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class CronBackgroundService : BackgroundService
{
    public CronBackgroundService(ContextService contextService) : base(contextService){}

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        Console.WriteLine($"CronBackgroundService executed at {DateTime.Now}");
    }
}
