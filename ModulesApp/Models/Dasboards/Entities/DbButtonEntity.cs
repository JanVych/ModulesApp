using ModulesApp.Helpers;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbButtonEntity : DbDashboardEntity
{
    public string Title = string.Empty;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        LoadState();
    }

    public override void LoadState()
    {
        if (Data.TryGetValue("Title", out var t))
        {
            Title = DataConvertor.ToString(t);
        }
    }

    public override void SaveToData()
    {
        Data["Title"] = Title;
    }
}
