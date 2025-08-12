using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbKeyValueEntity: DbDashboardEntity  
{
    public string Title = string.Empty;
    public string Value = string.Empty;
    public string Suffix = string.Empty;
    [NotMapped]
    public string Icon { get; set; } = string.Empty;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        LoadState();
    }

    public override void LoadState()
    {
        Title = Data.TryGetValue("Title", out var tv) == true ? tv?.ToString() ?? "" : Title;
        Value = Data.TryGetValue("Value", out var vv) == true ? vv?.ToString() ?? "" : Value;
        Suffix = Data.TryGetValue("Suffix", out var sv) == true ? sv?.ToString() ?? "" : Suffix;
        Icon = Data.TryGetValue("Icon", out var iv) == true ? iv?.ToString() ?? "" : Icon;
    }

    public override void SaveToData()
    {
        Data["Title"] = Title;
        Data["Suffix"] = Suffix;
        Data["Icon"] = Icon;
    }
}
