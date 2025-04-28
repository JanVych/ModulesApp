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
        Data = data;
        DisplayValue = Data.TryGetValue("Value", out var vv) == true ? vv?.ToString() ?? "" : "";
        InputValue = Data.TryGetValue("InputValue", out var iv) == true ? iv?.ToString() ?? "" : "";
        Title = Data.TryGetValue("Title", out var title) == true ? title?.ToString() ?? "" : "";
    }

    public override void SaveData()
    {
        Data["InputValue"] = InputValue;
        Data["DisplayValue"] = DisplayValue;
        Data["Title"] = Title;
    }
}
