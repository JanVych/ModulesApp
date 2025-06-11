using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class CronBackgroundService : BackgroundService
{
    public CronBackgroundService(ContextService contextService) : base(contextService){}

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        var name = context.JobDetail.Key.Name;
        Console.WriteLine($"CronBackgroundService id: {name}, time: {DateTime.Now}");
    }
}
