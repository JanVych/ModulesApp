using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbValueSetterEntity: DbDashboardEntity
{
    [NotMapped]
    public string Title { get; set; } = string.Empty;
    [NotMapped]
    public string DisplayValue { get; set; } = "0";
    [NotMapped]
    public string InputValue { get; set; } =  "0";

    public override void UpdateData(Dictionary<string, object> data)
    {
        foreach (var (key, value) in data)
        {
            Data[key] = value;
        }
        DisplayValue = Data.TryGetValue("Value", out var vv) == true ? vv?.ToString() ?? "" : DisplayValue;
        InputValue = Data.TryGetValue("InputValue", out var iv) == true ? iv?.ToString() ?? "" : InputValue;
        Title = Data.TryGetValue("Title", out var title) == true ? title?.ToString() ?? "" : Title;
    }

    public override void SaveData()
    {
        Data["InputValue"] = InputValue;
        Data["Value"] = DisplayValue;
        Data["Title"] = Title;
    }
}
