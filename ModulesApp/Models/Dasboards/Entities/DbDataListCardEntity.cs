using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbDataListCardEntity : DbDashboardEntity
{
    public class TableItem
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    [NotMapped]
    public List<TableItem> TableData = [];

    public override void UpdateData(Dictionary<string, object> data)
    {
        foreach(var (key, value) in data)
        {
            Data[key] = value;
        }
        if (Data.TryGetValue("Titles", out var titles))
        {
            Data.TryGetValue("Values", out var values);
            var titlesList = ToStringList(titles);
            var valuesList = ToStringList(values);

            TableData = titlesList?
                .Select((title, index) => new TableItem
                {
                    Title = title,
                    Value = index < valuesList?.Count ? valuesList[index] : string.Empty
                })
                .ToList() ?? [];
        }
    }

    private static List<string>? ToStringList(object? value)
    {
        if (value is List<string> eValue)
        {
            return eValue;
        }
        if (value is JsonElement jValue && jValue.ValueKind == JsonValueKind.Array)
        {
            return jValue
                .EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<string>(e.GetRawText()) ?? string.Empty)
                .ToList();
        }
        return null;
    }

    public override void SaveData()
    {
        Data["Titles"] = TableData.Select(i => i.Title).ToList();
        Data["Values"] = TableData.Select(i => i.Value).ToList();
    }
}
