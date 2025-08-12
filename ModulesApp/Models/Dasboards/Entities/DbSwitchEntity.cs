using ModulesApp.Helpers;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbSwitchEntity : DbDashboardEntity
{
    public bool Value = false;
    public string Title = string.Empty;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        LoadState();
    }

    public override void LoadState()
    {
        if( Data.TryGetValue("Value", out var v))
        {
            Value = DataConvertor.ToBool(v);
        }
        if (Data.TryGetValue("Title", out var t))
        {
            Title = DataConvertor.ToString(t);
        }
    }

    public override void SaveToData()
    {
        Data["Value"] = Value;
        Data["Title"] = Title;
    }
}
