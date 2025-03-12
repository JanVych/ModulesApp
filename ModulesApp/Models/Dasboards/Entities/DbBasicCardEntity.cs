using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbBasicCardEntity: DbDashboardEntity  
{
    private string _title = string.Empty;
    [NotMapped]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            Data["Title"] = value;
        }

    }

    [NotMapped]
    public string Value { get; set; } = string.Empty;

    public override void UpdateData(Dictionary<string, object> data)
    {
        Data = data;
        Title = Data.TryGetValue("Title", out var tv) == true ? tv?.ToString() ?? "" : "";
        Value = Data.TryGetValue("Value", out var vv) == true ? vv?.ToString() ?? "" : "";
    }

    public override void SaveData()
    {
        Data["Title"] = Title;
    }
}
