namespace ModulesApp.Services;

public class NotifyService
{
    public event Action? BackgroundServiceChangedEvent;
    public event Action<(long EntityId, string Key, object? Value)>? DashboardEntityDataEvent;
    public event Action? ModulesDbChangedEvent;

    public void NotifyBackgroundServiceChanged()
    {
        BackgroundServiceChangedEvent?.Invoke();
    }

    public void NotifyDashboardEntityDataChanged(long entityId , string key, object? value)
    {
        DashboardEntityDataEvent?.Invoke((entityId, key, value));
    }

    public void NotifyModulesChanged()
    {
        ModulesDbChangedEvent?.Invoke();
    }
}
