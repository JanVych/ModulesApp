using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.BackgroundServices.Servicves;
using ModulesApp.Services.Data;
using Quartz;

namespace ModulesApp.Services;

public class BackgroundServiceManager
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly BackgroundServiceService _backgroundService;

    public BackgroundServiceManager(ISchedulerFactory schedulerFactory, BackgroundServiceService backgroundService)
    {
        _schedulerFactory = schedulerFactory;
        _backgroundService = backgroundService;
    }

    public async Task LaunchAsync()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var services = await _backgroundService.GetAllAsync();
        foreach (var s in services)
        {
            if(s.Status == BackgroundServiceStatus.Active)
            {
                await ScheduleJobAsync(s, scheduler);
            }
        }
    }

    private async Task ScheduleJobAsync(DbBackgroundService service, IScheduler? scheduler = null)
    {
        scheduler ??= await _schedulerFactory.GetScheduler();
        if (await scheduler.CheckExists(new JobKey(service.Id.ToString(), "DefaultGroup")))
        {
            return;
        }
        JobBuilder builder = service.Type switch
        {
            BackgroundServiceType.Cron => JobBuilder.Create<CronBackgroundService>(),
            BackgroundServiceType.Goodwe => JobBuilder.Create<GoodweBackgroundService>(),
            BackgroundServiceType.Http => JobBuilder.Create<HttpBackgroundService>(),
            BackgroundServiceType.OteElectricityDam => JobBuilder.Create<OteElectricityDamBacgroundService>(),
            _ => JobBuilder.Create<CronBackgroundService>()
        };
        var newJob = builder.WithIdentity(service.Id.ToString(), "DefaultGroup").Build();

        var neWtrigger = TriggerBuilder.Create()
            .WithIdentity(service.Id.ToString())
            .ForJob(newJob)
            .WithCronSchedule(service.CronExpression)
            .Build();

        await scheduler.ScheduleJob(newJob, neWtrigger);
    }

    public async Task CreateServiceAsync(DbBackgroundService service)
    {
        service.Status = BackgroundServiceStatus.Active;
        await _backgroundService.AddAsync(service);
        await ScheduleJobAsync(service);
    }

    public async Task StartServiceAsync(DbBackgroundService service, IScheduler? scheduler = null)
    {
        scheduler ??= await _schedulerFactory.GetScheduler();
        if (await scheduler.CheckExists(new JobKey(service.Id.ToString(), "DefaultGroup")))
        {
            var state = await scheduler.GetTriggerState(new TriggerKey(service.Id.ToString()));
            if (state == TriggerState.Paused)
            {
                await scheduler.ResumeJob(new JobKey(service.Id.ToString(), "DefaultGroup"));
            }
        }
        else
        {
            await ScheduleJobAsync(service, scheduler);
        }
        service.Status = BackgroundServiceStatus.Active;
        await _backgroundService.UpdateAsync(service);
    }

    public async Task StopServiceAsync(DbBackgroundService service)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (await scheduler.CheckExists(new JobKey(service.Id.ToString(), "DefaultGroup")))
        {
            await scheduler.PauseJob(new JobKey(service.Id.ToString(), "DefaultGroup"));
        }
        service.Status = BackgroundServiceStatus.Paused;
        await _backgroundService.UpdateAsync(service);
    }

    public async Task DeleteServiceAsync(DbBackgroundService service)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (await scheduler.CheckExists(new JobKey(service.Id.ToString(), "DefaultGroup")))
        {
            await scheduler.DeleteJob(new JobKey(service.Id.ToString(), "DefaultGroup"));
        }
        await _backgroundService.DeleteAsync(service);
    }

    public async Task UpdateServiceAsync(DbBackgroundService service)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (await scheduler.CheckExists(new JobKey(service.Id.ToString(), "DefaultGroup")))
        {
            await scheduler.DeleteJob(new JobKey(service.Id.ToString(), "DefaultGroup"));
        }
        await _backgroundService.UpdateAsync(service);
        if (service.Status == BackgroundServiceStatus.Active)
        {
            await ScheduleJobAsync(service, scheduler);
        }
    }
}
