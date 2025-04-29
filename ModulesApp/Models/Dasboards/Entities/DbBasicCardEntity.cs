using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbBasicCardEntity: DbDashboardEntity  
{
    [NotMapped]
    public string Title { get; set; } = string.Empty;

    [NotMapped]
    public string Value { get; set; } = string.Empty;

    public override void UpdateData(Dictionary<string, object> data)
    {
        foreach (var (key, value) in data)
        {
            Data[key] = value;
        }
        Title = Data.TryGetValue("Title", out var tv) == true ? tv?.ToString() ?? "" : Title;
        Value = Data.TryGetValue("Value", out var vv) == true ? vv?.ToString() ?? "" : Value;
    }

    public override void SaveData()
    {
        Data["Title"] = Title;
    }
}
