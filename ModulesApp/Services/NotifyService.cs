using ModulesApp.Models.Dasboards;

namespace ModulesApp.Services;

public class NotifyService
{
    public event Action? BackgroundServiceChangedEvent;
    public event Action<DbDashboardEntity>? DashboardEntityDataEvent;
    public event Action? ModulesDbChangedEvent;

    public void NotifyBackgroundServiceChanged()
    {
        BackgroundServiceChangedEvent?.Invoke();
    }

    public void NotifyDashboardEntityDataChanged(DbDashboardEntity entity)
    {
        DashboardEntityDataEvent?.Invoke(entity);
    }

    public void NotifyModulesChanged()
    {
        ModulesDbChangedEvent?.Invoke();
    }
}
