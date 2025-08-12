using ModulesApp.Helpers;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbValueSetterEntity: DbDashboardEntity
{
    public string Title = string.Empty;
    public string CurrentValue = "0";
    public string TargetValue =  "0";

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        LoadState();
    }

    public override void LoadState()
    {
        if (Data.TryGetValue("Title", out var ti))
        {
            Title = DataConvertor.ToString(ti);
        }
        if (Data.TryGetValue("CurrentValue", out var cu))
        {
            CurrentValue = DataConvertor.ToString(cu);
        }
        if (Data.TryGetValue("TargetValue", out var tg))
        {
            TargetValue = DataConvertor.ToString(tg);
        }
    }

    public override void SaveToData()
    {
        Data["CurrentValue"] = CurrentValue;
        Data["TargetValue"] = TargetValue;
        Data["Title"] = Title;
    }
}
