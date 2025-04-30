using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbDataListEntity : DbDashboardEntity
{
    public class TableItem
    {
        public string? Column1 { get; set; } = string.Empty;
        public string? Column2 { get; set; } = string.Empty;
    }

    [NotMapped]
    public List<TableItem> TableData = [];

    public override void UpdateData(Dictionary<string, object?> data)
    {
        foreach(var (key, value) in data)
        {
            Data[key] = value;
        }
        if (Data.TryGetValue("Column1", out var titles))
        {
            Data.TryGetValue("Value", out var values);
            var titlesList = DataConvertor.ToList<string>(titles);
            var valuesList = DataConvertor.ToList<string>(values);

            TableData = titlesList?
                .Select((title, index) => new TableItem
                {
                    Column1 = title,
                    Column2 = index < valuesList?.Count ? valuesList[index] : string.Empty
                })
                .ToList() ?? [];
        }
    }

    public override void SaveData()
    {
        Data["Column1"] = TableData.Select(i => i.Column1).ToList();
        Data["Value"] = TableData.Select(i => i.Column2).ToList();
    }
}
